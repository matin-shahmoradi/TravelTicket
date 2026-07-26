namespace BuildingBlocks.Messaging.Events.OrderEvents
{
    public sealed class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public decimal Amount { get; init; }
    }
}
