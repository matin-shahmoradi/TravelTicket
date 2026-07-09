namespace Payment.api.Domain.StrongIdTypes
{
    [StronglyTypedId]
    public partial struct CustomerId
    {
        public static CustomerId Of(Guid customerId) => new CustomerId(customerId);
    }
}
