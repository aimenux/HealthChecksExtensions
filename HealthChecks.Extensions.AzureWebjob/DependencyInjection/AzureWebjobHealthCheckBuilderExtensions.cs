using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Extensions.AzureWebjob.DependencyInjection
{
    public static class AzureWebjobHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddAzureWebjob(
            this IHealthChecksBuilder builder,
            AzureWebjobHealthCheckOptions options,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default)
        {
            return builder.Add(new HealthCheckRegistration(
                options.HealthCheckName,
                sp => new AzureWebjobHealthCheck(options),
                failureStatus,
                tags,
                timeout));
        }
    }
}
