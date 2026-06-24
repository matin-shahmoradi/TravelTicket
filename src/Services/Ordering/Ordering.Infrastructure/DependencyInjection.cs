using BuildingBlocks.Behaviors;
using BuildingBlocks.EntityFramwork.Interceptors;
using BuildingBlocks.Messaging.Events;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Data;
using Ordering.Infrastructure.Data;
using System.Reflection;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderDbContext, OrderDbContext>();
            services.AddScoped<ISaveChangesInterceptor, AuditInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
            //services.AddMassTransitWithAssembly(configuration, typeof(BasketCheckOutEventConsumer));
            services.AddMassTransitWithAssembly(configuration, Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            services.AddDbContext<OrderDbContext>((sp, opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            })
                .AddNpgsql<OrderDbContext>(configuration.GetConnectionString("PostgresDefaultConnection"));

            return services;
        }
    }
}
