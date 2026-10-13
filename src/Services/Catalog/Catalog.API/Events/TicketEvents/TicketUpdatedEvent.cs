namespace Catalog.API.Events.TicketEvents
{
    public record TicketUpdatedEvent(Ticket UpdatedTicket) : IDomainEvent;
}
