using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BuildingBlocks.Extensions
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseSharedSerilog(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });
        }
    }
}
