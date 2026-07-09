namespace Payment.api.Domain.Enums
{
    public enum PaymentStatus
    {
        None = 0,
        Succeeded = 1,
        GatewayRequested = 2,
        Pending = 3,
        Failed = 4,
        Cancelled = 5
    }
}
