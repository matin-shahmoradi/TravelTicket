using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Interceptors;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<OrderContext>(opt =>
            {
                opt.AddInterceptors(new AuditableEntityInterceptor());
            })
                .AddNpgsql<OrderContext>(configuration.GetConnectionString("PostgresDefaultConnection"));

            return services;
        }
    }
}
