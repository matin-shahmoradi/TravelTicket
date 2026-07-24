namespace BuildingBlocks.Messaging.Events
{
    public abstract class IntegrationEvent : IIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccuredOn { get; init; } = DateTime.UtcNow;
        public string EventType => GetType().AssemblyQualifiedName!;
    }
}
