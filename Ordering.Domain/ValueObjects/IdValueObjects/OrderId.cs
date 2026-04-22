namespace Ordering.Domain.ValueObjects.IdValueObjects
{
    //public readonly record struct OrderId(Guid Value)
    //{
    //    public static OrderId New() => new OrderId(Guid.NewGuid());
    //}

    [StronglyTypedId]
    public partial struct OrderId()
    {
        public static OrderId New(Guid value) => new OrderId(value);
    }
}
