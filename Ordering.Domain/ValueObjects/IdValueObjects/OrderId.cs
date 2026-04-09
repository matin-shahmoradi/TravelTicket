namespace Ordering.Domain.ValueObjects.IdValueObjects
{
    public readonly record struct OrderId(Guid Value)
    {
        public static OrderId New() => new OrderId(Guid.NewGuid());
    }
}
