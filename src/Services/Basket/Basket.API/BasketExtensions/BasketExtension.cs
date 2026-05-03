using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Basket.API.BasketExtensions
{
    public static class BasketExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services , IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            services.AddDbContext<BacketDbContext>(cfg =>
            {
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddStackExchangeRedisCache(config =>
            {
                config.Configuration = configuration.GetConnectionString("Redis");
                config.InstanceName = "BasketRedis";
            });

            services.AddScoped<IBasketRepository, BasketRepository>();
            services.Decorate<IBasketRepository, CachedBasketRepository>();

            services.AddCarter();
            services.AddValidatorsFromAssembly(assembly);
            services.AddSwaggerGen();
            services.AddProblemDetails();

            return services;
        }

        public static async Task MigrateDatabase(this WebApplication app)
        {
            using var scoped = app.Services.CreateScope();
            var context = scoped.ServiceProvider.GetRequiredService<BacketDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
