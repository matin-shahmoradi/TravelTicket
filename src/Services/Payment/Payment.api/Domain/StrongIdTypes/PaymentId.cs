namespace Payment.api.Domain.StrongIdTypes
{
    [StronglyTypedId]
    public partial struct PaymentId
    {
        public static PaymentId Of(Guid id) => new(id);
    }
}
