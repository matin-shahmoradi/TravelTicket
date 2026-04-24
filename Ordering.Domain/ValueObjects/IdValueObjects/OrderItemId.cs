namespace Ordering.Domain.ValueObjects.IdValueObjects
{
    //public readonly record struct OrderItemId(Guid Value)
    //{
    //    public static OrderItemId New() => new OrderItemId(Guid.NewGuid());
    //}

    [StronglyTypedId]
    public partial struct OrderItemId() 
    {
        public static OrderItemId New(Guid value) => new OrderItemId(value);
    }
}
