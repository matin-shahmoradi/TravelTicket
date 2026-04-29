namespace Catalog.API.Events
{
    public record TicketUpdatedEvent(Ticket UpdatedTicket) : IDomainEvent;
}
