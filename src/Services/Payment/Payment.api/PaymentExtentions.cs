using BuildingBlocks.Abstractions;
using BuildingBlocks.Behaviors;
using BuildingBlocks.EntityFramwork;
using BuildingBlocks.EntityFramwork.Interceptors;
using BuildingBlocks.Infrastracture.CorrelationId;
using BuildingBlocks.Messaging.Events;
using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Payment.api.Data;
using Payment.api.Options;
using Payment.api.Repositories;
using Payment.api.Services;
using System.Reflection;

namespace Payment.api
{
    public static class PaymentExtentions
    {
        public static IServiceCollection PaymentServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.Configure<ZarinPalOptions>(configuration.GetSection("ZarinPal"));

            services.AddDbContext<PaymentDbContext>((sp, cfg) =>
            {
                var interceptors = sp.GetServices<ISaveChangesInterceptor>();
                cfg.AddInterceptors(interceptors);
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            services.AddMassTransitWithAssembly(
               configuration: configuration,
               assembly: assembly);

            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddCorrelationId();
            services.AddSwaggerGen();
            services.AddCarter();

            services.AddScoped<ITransactionExecutor, EfTransactionExecutor<PaymentDbContext>>();
            services.AddScoped<ISaveChangesInterceptor, AuditInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IZarinPalGateway, ZarinPalGateway>();
            return services;
        }

        public static WebApplication UsePayments(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCorrelationId();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.MapCarter();
            app.MapControllers();
            return app;
        }
    }
}
