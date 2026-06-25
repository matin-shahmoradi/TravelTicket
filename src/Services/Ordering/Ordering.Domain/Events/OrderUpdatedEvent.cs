using BuildingBlocks.DDD;
using Ordering.Domain.Models;

namespace Ordering.Domain.Events
{
    public record OrderUpdatedEvent(Order Order) : IDomainEvent;

}
