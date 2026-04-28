namespace BuildingBlocks.DDD
{
    public interface IAggregate<T> : IEntity<T>
    {

    }
    public interface IAggregate : IEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvent();
    }
}
