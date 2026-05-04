using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.Events.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithAssembly(this IServiceCollection services , Assembly assembly)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.SetInMemorySagaRepositoryProvider();

                cfg.AddConsumers(assembly);

                cfg.AddSagaStateMachines(assembly);

                cfg.AddSagas(assembly);

                cfg.AddActivities(assembly);

                cfg.UsingInMemory((context, configurator) =>
                {
                    configurator.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
