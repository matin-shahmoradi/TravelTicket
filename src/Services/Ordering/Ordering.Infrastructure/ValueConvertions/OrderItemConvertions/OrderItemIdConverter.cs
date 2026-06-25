using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Infrastructure.ValueConvertions.OrderItemConvertions
{
    internal sealed class OrderItemIdConverter : ValueConverter<OrderItemId, Guid>
    {
        public OrderItemIdConverter() :
            base(
                id => id.Value,
                value => OrderItemId.New())
        { }
    }
}
