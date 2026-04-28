using Catalog.API.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.CatalogExtensions
{
    public static class CatalogExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddScoped<ICatalogDbContext, CatalogDbContext>();

            services.AddDbContext<CatalogDbContext>(cfg =>
            {
                cfg.AddInterceptors(new AuditInterceptor());
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
