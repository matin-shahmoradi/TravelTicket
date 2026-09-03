namespace Catalog.API.Events
{
    public sealed record TicketDeletedEvent(Guid TicketId) : IDomainEvent;
}
