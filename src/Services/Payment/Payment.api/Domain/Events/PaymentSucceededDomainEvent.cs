using BuildingBlocks.DDD;

namespace Payment.api.Domain.Events
{
    public sealed record PaymentSucceededDomainEvent(
        Guid PaymentId,
        Guid OrderId,
        Guid CustomerId,
        string PaymentStatus,
        decimal Amount,
        string Authority,
        string TransactionId,
        DateTime PaidAtUtc,
        DateTime OccurredOnUtc
    ) : IDomainEvent;
}
