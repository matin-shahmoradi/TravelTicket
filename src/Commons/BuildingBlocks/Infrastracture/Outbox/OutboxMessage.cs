using BuildingBlocks.Messaging.Events;
using System.Text.Json;

namespace BuildingBlocks.Infrastracture.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public required string Type { get; set; }
        public required string Content { get; set; }

        public DateTime OccuredOnUtc { get; set; }

        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }

        public static OutboxMessage CreateOutboxMessage(IntegrationEvent @event)
        {
            return new OutboxMessage
            {
                Id = @event.EventId,
                Type = @event.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(@event),
                OccuredOnUtc = @event.OccuredOn,
            };
        }
    }
}
