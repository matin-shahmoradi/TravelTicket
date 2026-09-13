using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddTelemetry(this IServiceCollection services, string serviceName)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddNpgsqlInstrumentation();
                    metrics.AddOtlpExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddNpgsql();
                    tracing.AddOtlpExporter();
                })
                .WithLogging(logging => logging.AddOtlpExporter());

            return services;
        }
    }
}
