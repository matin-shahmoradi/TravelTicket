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
            return services;
        }

        public static WebApplication UseApiService(this WebApplication app)
        {
            return app;
        }
    }
}
