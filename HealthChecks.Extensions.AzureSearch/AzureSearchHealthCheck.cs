using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Extensions.AzureSearch
{
    public class AzureSearchHealthCheck : IHealthCheck
    {
        private readonly AzureSearchHealthCheckOptions _options;

        public AzureSearchHealthCheck(AzureSearchHealthCheckOptions options)
        {
            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, description: $"{nameof(AzureSearchHealthCheck)} execution is cancelled.");
                }

                var statistics = await GetServiceStatisticsAsync(cancellationToken);
                if (statistics.UsagePercentage == null)
                {
                    return HealthCheckResult.Degraded("Unable to get azure search usage statistics");
                }

                var currentUsagePercentage = statistics.UsagePercentage.Value;
                return currentUsagePercentage >= _options.ThresholdUsagePercentage
                    ? HealthCheckResult.Unhealthy($"Azure search usage percentage {currentUsagePercentage} exceeds threshold {_options.ThresholdUsagePercentage}")
                    : HealthCheckResult.Healthy($"AzureSearch statistics {statistics}");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private async Task<AzureSearchStatistics> GetServiceStatisticsAsync(CancellationToken cancellationToken)
        {
            var indexName = _options.SearchIndexClient.IndexName;
            var serviceStats = await _options.SearchServiceClient.GetServiceStatisticsAsync(cancellationToken: cancellationToken);
            var indexStats = await _options.SearchServiceClient.Indexes.GetStatisticsAsync(indexName, cancellationToken: cancellationToken);
            
            var quotaBytes = serviceStats.Counters?.StorageSizeCounter?.Quota;

            return new AzureSearchStatistics
            {
                DocumentsNumber = indexStats.DocumentCount,
                UsageBytes = indexStats.StorageSize,
                QuotaBytes = quotaBytes
            };
        }
    }
}
