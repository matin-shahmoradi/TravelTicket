namespace Catalog.API.Events.TicketEvents
{
    public record TicketCreatedEvent(Ticket Ticket) : IDomainEvent;
}
