using System;
using System.Net.Http;
using HealthChecks.Extensions.AzureWebjob.Models;

namespace HealthChecks.Extensions.AzureWebjob
{
    public class AzureWebjobHealthCheckOptions
    {
        public string HealthCheckName { get; } = "Azure webjob healthcheck";

        public IHttpClientFactory HttpClientFactory { get; }

        public AzureWebjobSettings AzureWebjobSettings { get; }

        public Predicate<AzureWebjobDetails> HealthCheckPredicate { get; }

        public AzureWebjobHealthCheckOptions(
            IHttpClientFactory httpClientFactory,
            AzureWebjobSettings azureWebjobSettings,
            Predicate<AzureWebjobDetails> healthCheckPredicate)
        {
            HttpClientFactory = httpClientFactory;
            AzureWebjobSettings = azureWebjobSettings;
            HealthCheckPredicate = healthCheckPredicate;
        }
    }
}