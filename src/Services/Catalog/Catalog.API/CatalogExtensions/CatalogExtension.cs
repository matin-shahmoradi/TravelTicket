using BuildingBlocks.EntityFramwork.Interceptors;
using BuildingBlocks.Messaging.Events.Extensions;
using Catalog.API.EventHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Catalog.API.CatalogExtensions
{
    public static class CatalogExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<TicketCreatedEventHandler>();
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<ICatalogDbContext, CatalogDbContext>();
            services.AddScoped<ISaveChangesInterceptor ,AuditInterceptor>();
            services.AddScoped<ISaveChangesInterceptor ,DispatchDomainEventInterceptor>();

            services.AddDbContext<CatalogDbContext>((sp,cfg) =>
            {
                cfg.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddMassTransitWithAssembly(configuration,Assembly.GetExecutingAssembly());
            return services;
        }
        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            return result.Error!.Value.ErrorType switch
            {
                ErrorType.VALIDATION_ERROR => Results.BadRequest(result.Error),
                ErrorType.NOT_FOUND => Results.NotFound(result.Error),
                _ => Results.Problem(result.Error.Value.Message)
            };
        }
    }
}
