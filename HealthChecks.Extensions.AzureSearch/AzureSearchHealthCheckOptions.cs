using System;
using Microsoft.Azure.Search;

namespace HealthChecks.Extensions.AzureSearch
{
    public class AzureSearchHealthCheckOptions
    {
        public string Name { get; }
        public int ThresholdUsagePercentage { get; }
        public ISearchIndexClient SearchIndexClient { get; }
        public ISearchServiceClient SearchServiceClient { get; }
        
        public static readonly int DefaultThresholdUsagePercentage = 90;

        public AzureSearchHealthCheckOptions(
            ISearchServiceClient searchServiceClient,
            ISearchIndexClient searchIndexClient) : this(searchServiceClient, searchIndexClient, DefaultThresholdUsagePercentage)
        {
        }

        public AzureSearchHealthCheckOptions(
            ISearchServiceClient searchServiceClient,
            ISearchIndexClient searchIndexClient,
            int thresholdUsagePercentage)
        {
            const int min = 0;
            const int max = 100;

            if (thresholdUsagePercentage < min || thresholdUsagePercentage > max)
            {
                throw new ArgumentOutOfRangeException(nameof(thresholdUsagePercentage), $"percentage should be between {min} and {max}");
            }

            Name = "Azure search healthcheck";
            SearchIndexClient = searchIndexClient;
            SearchServiceClient = searchServiceClient;
            ThresholdUsagePercentage = thresholdUsagePercentage;
        }
    }
}