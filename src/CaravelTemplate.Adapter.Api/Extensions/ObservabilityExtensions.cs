using CaravelTemplate.Application.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CaravelTemplate.Adapter.Api.Extensions;

// Useful documentation: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel
// The .NET OTel implementation uses these platform APIs for instrumentation:
// Microsoft.Extensions.Logging.ILogger<TCategoryName> for logging
// System.Diagnostics.Metrics.Meter for metrics
// System.Diagnostics.ActivitySource and System.Diagnostics.Activity for distributed tracing

public static class ObservabilityExtensions
{
    public static void AddOpenTelemetry(this IHostApplicationBuilder builder)
    {   
        // We are using Serilog OpenTelemetry Sink
        // builder.Logging.AddOpenTelemetry(options =>
        // {
        //     options.IncludeScopes = true;
        //     options.IncludeFormattedMessage = true;
        // });

        builder.Services.AddOpenTelemetry()
            // Configure OpenTelemetry Resources with the application name
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: ObservabilityTags.ApplicationName,
                    serviceVersion: "1.0.0",
                    serviceInstanceId: Environment.MachineName
                ).AddAttributes([
                    new KeyValuePair<string, object>("environment", builder.Environment.EnvironmentName)
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
                
                if (builder.Environment.IsDevelopment())
                {
                    tracing.SetSampler<AlwaysOnSampler>();
                }
            });

        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(options => { options.AddOtlpExporter(); });
            builder.Services.ConfigureOpenTelemetryMeterProvider((provider) => { provider.AddOtlpExporter(); });
            builder.Services.ConfigureOpenTelemetryTracerProvider((provider) => { provider.AddOtlpExporter(); });
        }
        else
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(options => { options.AddConsoleExporter(); });
            builder.Services.ConfigureOpenTelemetryMeterProvider((provider) => { provider.AddConsoleExporter(); });
            builder.Services.ConfigureOpenTelemetryTracerProvider((provider) => { provider.AddConsoleExporter(); });
        }
        
        // Configure Metrics services
        builder.Services.AddSingleton<BookMetrics>();
    }
}