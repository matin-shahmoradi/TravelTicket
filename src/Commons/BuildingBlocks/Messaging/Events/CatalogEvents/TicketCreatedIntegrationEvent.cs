namespace BuildingBlocks.Messaging.Events.CatalogEvents
{
    public class TicketCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid TicketId { get; init; }
        public string Origin { get; init; } = default!;
        public string Destination { get; init; } = default!;
        public string Description { get; init; } = default!;
        public DateTime TravelDate { get; init; }
        public decimal Price { get; init; }
    }
}
