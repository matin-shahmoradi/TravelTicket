using Catalog.API.Data.Interceptors;
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
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<ICatalogDbContext, CatalogDbContext>();
            services.AddScoped<ISaveChangesInterceptor ,AuditInterceptor>();
            services.AddScoped<ISaveChangesInterceptor ,DispatchEventInterceptor>();

            services.AddDbContext<CatalogDbContext>((sp,cfg) =>
            {
                cfg.AddInterceptors(sp.GetRequiredService<ISaveChangesInterceptor>());
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
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
