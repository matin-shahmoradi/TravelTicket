using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddTelemetry(this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("TravelTicket"))
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();

                    metrics.AddOtlpExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();

                    tracing.AddOtlpExporter();
                })
                .WithLogging(logging => logging.AddOtlpExporter());

            return services;
        }
    }
}
