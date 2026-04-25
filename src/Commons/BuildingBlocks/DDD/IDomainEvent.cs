namespace BuildingBlocks.DDD
{
    public interface IDomainEvent : INotification
    {
        Guid EventId => Guid.NewGuid();
        public DateTime OccurredOn => DateTime.UtcNow;
        public string eventType => GetType().AssemblyQualifiedName!;
    }
}
