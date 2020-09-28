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
            _httpClient.BaseAddress = new Uri(_options.AzureWebjobSettings.Url);
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

                var settings = _options.AzureWebjobSettings;
                var details = await GetWebJobDetailsAsync();
                if (details == null)
                {
                    return HealthCheckResult.Degraded($"Unable to get azure webjob details for {settings}");
                }

                return _options.HealthCheckPredicate(details) 
                    ? HealthCheckResult.Healthy($"Azure webjob {settings} is up")
                    : HealthCheckResult.Unhealthy($"Azure webjob {settings} is down");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private async Task<AzureWebjobDetails> GetWebJobDetailsAsync()
        {
            var webjobRelativeUrl = _options.AzureWebjobSettings.RelativeWebjobUrl;

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
            var settings = _options.AzureWebjobSettings;
            var bytes = Encoding.ASCII.GetBytes($"{settings.UserName}:{settings.Password}");
            var base64 = Convert.ToBase64String(bytes);
            return new AuthenticationHeaderValue("Basic", base64);
        }
    }
}
