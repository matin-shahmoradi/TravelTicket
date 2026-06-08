using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastracture.CorrelationId
{
    public static class CorrelationIdExtensions
    {
        public static IServiceCollection AddCorrelationId(this IServiceCollection services)
        {
            services.AddScoped<CorrelationIdHandler>();
            return services;
        }
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }

    }
}
