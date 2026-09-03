namespace BuildingBlocks.Messaging.Events.CatalogEvents
{
    public class TicketDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid TicketId { get; init; }
    }
}
