using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Data;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Interceptors;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderDbContext, OrderDbContext>();
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();

            services.AddDbContext<OrderDbContext>((sp,opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            })
                .AddNpgsql<OrderDbContext>(configuration.GetConnectionString("PostgresDefaultConnection"));

            return services;
        }
    }
}
