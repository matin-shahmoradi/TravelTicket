namespace Ordering.Domain.ValueObjects.IdValueObjects
{
    //public readonly record struct TicketId(Guid Value)
    //{
    //    public static TicketId New() => new TicketId(Guid.NewGuid());
    //}

    [StronglyTypedId]
    public partial struct TicketId()
    {
        public static TicketId New(Guid value) => new TicketId(value); 
    }
}
