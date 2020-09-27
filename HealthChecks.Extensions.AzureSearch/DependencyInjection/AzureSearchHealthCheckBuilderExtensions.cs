using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Extensions.AzureSearch.DependencyInjection
{
    public static class AzureSearchHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddAzureSearch(
            this IHealthChecksBuilder builder,
            AzureSearchHealthCheckOptions options,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default)
        {
            return builder.Add(new HealthCheckRegistration(
                options.Name,
                sp => new AzureSearchHealthCheck(options),
                failureStatus,
                tags,
                timeout));
        }
    }
}
