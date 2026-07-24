namespace Payment.api.Domain.Enums
{
    public enum PaymentStatus
    {
        None = 0,
        Pending = 1,
        GatewayRequested = 2,
        Succeeded = 3,
        Failed = 4,
        Cancelled = 5
    }
}
