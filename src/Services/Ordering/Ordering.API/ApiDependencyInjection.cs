using BuildingBlocks.Infrastracture.CorrelationId;
using BuildingBlocks.Infrastracture.Outbox.Extensions;
using Carter;
using System.Reflection;

namespace Ordering.API
{
    public static class ApiDependencyInjection
    {
        public static IServiceCollection AddApiService(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddCarter();
            services.AddHttpContextAccessor();
            services.AddCorrelationId();
            return services;
        }

        public static WebApplication UseApiService(this WebApplication app)
        {
            app.UseCorrelationId();
            app.UseBackgroundJobs("order-outbox-processor");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapCarter();
            return app;
        }
    }
}
