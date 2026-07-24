using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastracture.Outbox.Extensions
{
    public static class BackgroundJobExtensions
    {
        public static IApplicationBuilder UseBackgroundJobs(this WebApplication app, string jobId)
        {
            app.Services
                .GetRequiredService<IRecurringJobManager>()
                .AddOrUpdate<IOutboxProcessor>(
                    jobId,
                    job => job.ProcessMessageAsync(),
                    Cron.Minutely());
            return app;
        }
    }
}
