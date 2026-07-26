using BuildingBlocks.DDD;
using Payment.api.Domain.Enums;

namespace Payment.api.Domain.Events
{
    public record PaymentFailedDomainEvent(Guid OrderId, PaymentStatus PaymentStatus = PaymentStatus.Failed) : IDomainEvent;
}
