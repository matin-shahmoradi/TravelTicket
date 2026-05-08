namespace BuildingBlocks.DDD
{
    public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId> , IAggregate
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvents(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public IDomainEvent[] ClearDomainEvent()
        {
            IDomainEvent[] dequeuedEvents = _domainEvents.ToArray();

            _domainEvents.Clear();

            return dequeuedEvents;
        }
    }
}
