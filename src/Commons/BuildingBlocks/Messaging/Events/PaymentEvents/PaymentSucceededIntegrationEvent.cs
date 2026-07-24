namespace BuildingBlocks.Messaging.Events.PaymentEvents
{
    public sealed class PaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; init; }
        public string PaymentStatus { get; init; } = default!;
    }
}
