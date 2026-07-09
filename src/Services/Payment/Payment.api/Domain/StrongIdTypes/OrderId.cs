namespace Payment.api.Domain.StrongIdTypes
{
    [StronglyTypedId]
    public partial struct OrderId
    {
        public static OrderId Of(Guid orderId) => new OrderId(orderId);
    }
}
