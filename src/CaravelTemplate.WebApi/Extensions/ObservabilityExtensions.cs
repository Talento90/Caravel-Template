using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class ObservabilityExtensions
    {
        private static readonly Meter ApplicationMeter = new("CaravelTemplate", "1.0");
        public static readonly Counter<long> UserCreated = ApplicationMeter.CreateCounter<long>("UserCreated");

        public static void ConfigureObservability(this IServiceCollection services)
        {
            services.AddOpenTelemetryMetrics(builder =>
            {
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                builder.AddMeter("CaravelTemplate");
                builder.AddPrometheusExporter();
            });

            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
            });
        }
    }
}