using CaravelTemplate.Application.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CaravelTemplate.Host.Extensions;

// Useful documentation: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel
// The .NET OTel implementation uses these platform APIs for instrumentation:
// Microsoft.Extensions.Logging.ILogger<TCategoryName> for logging
// System.Diagnostics.Metrics.Meter for metrics
// System.Diagnostics.ActivitySource and System.Diagnostics.Activity for distributed tracing

public static class ObservabilityExtensions
{
    public static void AddOpenTelemetry(
        this IServiceCollection services,
        IConfigurationManager configuration,
        IHostEnvironment environment)
    {
        services.AddOpenTelemetry()
            // Configure OpenTelemetry Resources with the application name
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: ObservabilityTags.ApplicationName,
                    serviceVersion: "1.0.0",
                    serviceInstanceId: Environment.MachineName
                ).AddAttributes([
                    new KeyValuePair<string, object>("environment", environment.EnvironmentName)
                ])
            )

            // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
            .WithMetrics(metrics =>
                {
                    // Metrics provider from OpenTelemetry
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddMeter("Microsoft.EntityFrameworkCore")
                        .AddMeter("MassTransit")
                        .AddMeter(BookMetrics.MeterName);
                }
            )

            // Add Tracing for ASP.NET Core and our custom ActivitySource amd exporters
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource("MassTransit")
                    .AddSource(ObservabilityTags.ApplicationActivitySource);

                if (environment.IsDevelopment())
                {
                    tracing.SetSampler<AlwaysOnSampler>();
                }
            });

        var otlpEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            services.Configure<OpenTelemetryLoggerOptions>(options => { options.AddOtlpExporter(); });
            services.ConfigureOpenTelemetryMeterProvider((provider) => { provider.AddOtlpExporter(); });
            services.ConfigureOpenTelemetryTracerProvider((provider) => { provider.AddOtlpExporter(); });
        }
        else
        {
            services.Configure<OpenTelemetryLoggerOptions>(options => { options.AddConsoleExporter(); });
            services.ConfigureOpenTelemetryMeterProvider((provider) => { provider.AddConsoleExporter(); });
            services.ConfigureOpenTelemetryTracerProvider((provider) => { provider.AddConsoleExporter(); });
        }

        // Configure Metrics services
        services.AddSingleton<BookMetrics>();
    }
}