 using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.ValueConvertions.OrderConvertions
{
    internal sealed class OrderNameConverter : ValueConverter<OrderName, string>
    {
        public OrderNameConverter() :
            base(
                orderName => orderName.Value,
                value => new OrderName(value))
        { }
    }
}
