using BuildingBlocks.Messaging;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildingBlocks.Infrastracture.Outbox.Extensions
{
    public static class OutboxExtensions
    {
        public static IServiceCollection OutboxServices(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddHangfire(cfg =>
                cfg.UsePostgreSqlStorage(
                    options => options.UseNpgsqlConnection(connectionString)));

            services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));
            return services;
        }

        public static void AddIntegrationEvent(
            this DbSet<OutboxMessage> outboxMessages,
            IIntegrationEvent integrationEvent)
        {
            ArgumentNullException.ThrowIfNull(integrationEvent);

            var messageType = integrationEvent.GetType();

            var message = new OutboxMessage
            {
                Id = integrationEvent.EventId,
                Type = messageType.AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(integrationEvent, messageType),
                OccuredOnUtc = integrationEvent.OccuredOn,
            };

            outboxMessages.Add(message);
        }
    }
}
