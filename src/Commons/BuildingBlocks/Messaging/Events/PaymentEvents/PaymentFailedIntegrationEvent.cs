namespace BuildingBlocks.Messaging.Events.PaymentEvents
{
    public sealed class PaymentFailedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public string PaymentStatus { get; init; } = default!;
    }
}
