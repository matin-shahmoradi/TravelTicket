namespace Catalog.API.Domain.ValueObjects
{
    [StronglyTypedId]
    public partial struct TicketId()
    {
        public static TicketId New(Guid value) => new TicketId(value);
    }
}
