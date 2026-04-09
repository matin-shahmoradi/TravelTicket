namespace Ordering.Domain.ValueObjects.IdValueObjects
{
    public readonly record struct TicketId(Guid Value)
    {
        public static OrderItemId New() => new OrderItemId(Guid.NewGuid());
    }
}
