using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.Events.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithAssembly(
            this IServiceCollection services ,
            IConfiguration configuration,
            Assembly assembly)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddConsumers(assembly);
                cfg.AddSagas(assembly);
                cfg.AddSagaStateMachines(assembly);

                cfg.AddActivities(assembly);


                cfg.UsingRabbitMq((context , configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });
                    configurator.ConfigureEndpoints(context);  
                });
            });
            return services;
        }
    }
}
