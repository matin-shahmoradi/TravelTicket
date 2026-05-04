namespace Catalog.API.Events
{
    public record TicketPriceChangedEvent(Ticket ticket) : IDomainEvent;

}
