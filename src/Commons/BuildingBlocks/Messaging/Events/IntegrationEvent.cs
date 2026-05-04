namespace BuildingBlocks.Messaging.Events
{
    public class IntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccuredOn => DateTime.UtcNow;
        public string EventType => GetType().AssemblyQualifiedName!;

    }
}
