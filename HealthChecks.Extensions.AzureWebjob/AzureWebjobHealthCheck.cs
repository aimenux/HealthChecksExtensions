using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HealthChecks.Extensions.AzureWebjob.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace HealthChecks.Extensions.AzureWebjob
{
    public class AzureWebjobHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly AzureWebjobHealthCheckOptions _options;

        public AzureWebjobHealthCheck(AzureWebjobHealthCheckOptions options)
        {
            _options = options;
            _httpClient = _options.HttpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_options.AzureWebjobCredentials.Url);
            _httpClient.DefaultRequestHeaders.Authorization = GetAuthenticationHeader();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, description: $"{nameof(AzureWebjobHealthCheck)} execution is cancelled.");
                }

                var webjobName = _options.AzureWebjobCredentials.Name;

                var details = await GetWebJobDetailsAsync();
                if (details == null)
                {
                    return HealthCheckResult.Degraded($"Unable to get azure webjob details for {webjobName}");
                }

                return _options.HealthCheckPredicate(details) 
                    ? HealthCheckResult.Healthy($"Azure webjob {webjobName} is up")
                    : HealthCheckResult.Unhealthy($"Azure webjob {webjobName} is down");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private async Task<AzureWebjobDetails> GetWebJobDetailsAsync()
        {
            var webjobName = _options.AzureWebjobCredentials.Name;
            var webjobType = _options.AzureWebjobCredentials.Type;
            if (webjobType != null)
            {
                return await GetWebJobDetailsAsync(webjobName, webjobType.ToString());
            }

            var getTriggeredDetailsTask = GetWebJobDetailsAsync(webjobName, nameof(AzureWebjobType.Triggered));
            var getContinuousDetailsTask = GetWebJobDetailsAsync(webjobName, nameof(AzureWebjobType.Continuous));
            await Task.WhenAll(getTriggeredDetailsTask, getContinuousDetailsTask);
            return getTriggeredDetailsTask.Result ?? getContinuousDetailsTask.Result;
        }

        private async Task<AzureWebjobDetails> GetWebJobDetailsAsync(string webjobName, string webjobType)
        {
            var webjobRelativeUrl = BuildRelativeWebjobUrl(webjobName, webjobType);

            using (var httpResponseMessage = await _httpClient.GetAsync(webjobRelativeUrl))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<AzureWebjobDetails>(content);
                return details;
            }
        }

        private AuthenticationHeaderValue GetAuthenticationHeader()
        {
            var credentials = _options.AzureWebjobCredentials;
            var bytes = Encoding.ASCII.GetBytes($"{credentials.UserName}:{credentials.Password}");
            var base64 = Convert.ToBase64String(bytes);
            return new AuthenticationHeaderValue("Basic", base64);
        }

        private static string BuildRelativeWebjobUrl(string webjobName, string webjobType)
        {
            return $"/api/{webjobType}jobs/{webjobName}".ToLower();
        }
    }
}
