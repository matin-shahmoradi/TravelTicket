using Ordering.API.JsonConverter;
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

            services.ConfigureHttpJsonOptions(opt =>
            {
                opt.SerializerOptions.Converters.Add(new CustomerIdJsonConverter());
            });
            services.AddCarter();
            services.AddSwaggerGen();
            return services;
        }

        public static WebApplication UseApiService(this WebApplication app)
        {
            app.MapCarter();
            app.MapGet("/", ()=> "OrderApi");
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
