using BuildingBlocks.Messaging;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildingBlocks.Infrastracture.Outbox.Extensions
{
    public static class OutboxExtensions
    {
        public static IServiceCollection OutboxServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHangfire(cfg =>
                cfg.UsePostgreSqlStorage(
                    options => options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

            services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));
            return services;
        }

        public static void AddIntegrationEvent(
            this DbSet<OutboxMessage> outboxMessages,
            IIntegrationEvent integrationEvent)
        {
            ArgumentNullException.ThrowIfNull(integrationEvent);

            var messageType = integrationEvent.GetType();
            Console.WriteLine($"[OUTBOX-INTEGRATION] {messageType.AssemblyQualifiedName}");
            var message = new OutboxMessage
            {
                Id = integrationEvent.EventId,
                Type = messageType.AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(integrationEvent, messageType),
                OccuredOnUtc = integrationEvent.OccuredOn,
            };

            outboxMessages.Add(message);

            Console.WriteLine($"[OUTBOX-CREATED] {message.Type}");
            Console.WriteLine(Environment.StackTrace);
        }
    }
}
