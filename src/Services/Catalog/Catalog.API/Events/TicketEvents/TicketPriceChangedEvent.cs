namespace Catalog.API.Events.TicketEvents
{
    public record TicketPriceChangedEvent(Ticket ticket) : IDomainEvent;

}
