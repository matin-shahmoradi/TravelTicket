using BuildingBlocks.DDD;

namespace Payment.api.Domain.Events
{
    public record PaymentRequestedEvent(
        Guid PaymentId,
        string Authority,
        string Status) : IDomainEvent;

}
