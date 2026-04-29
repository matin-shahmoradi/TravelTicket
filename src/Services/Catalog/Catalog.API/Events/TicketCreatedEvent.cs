namespace Catalog.API.Events
{
    public record TicketCreatedEvent(Ticket Ticket) : IDomainEvent;
}
