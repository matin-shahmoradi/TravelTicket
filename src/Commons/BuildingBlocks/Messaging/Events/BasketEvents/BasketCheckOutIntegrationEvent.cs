namespace BuildingBlocks.Messaging.Events.BasketEvents
{
    public class BasketCheckOutIntegrationEvent : IntegrationEvent
    {
        public Guid CustomerId { get; set; }
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public List<BasketCheckOutIntegrationEventItem> Items = new();
    }

    public record BasketCheckOutIntegrationEventItem(Guid TicketId, int Quantity, decimal Price);
}
