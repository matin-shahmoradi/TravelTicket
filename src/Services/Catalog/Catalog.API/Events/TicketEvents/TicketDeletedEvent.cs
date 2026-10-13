namespace Catalog.API.Events.TicketEvents
{
    public sealed record TicketDeletedEvent(Guid TicketId) : IDomainEvent;
}
