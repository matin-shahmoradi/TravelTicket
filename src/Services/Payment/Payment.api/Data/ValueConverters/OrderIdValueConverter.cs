using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payment.api.Domain.StrongIdTypes;

namespace Payment.api.Data.ValueConverters
{
    public sealed class OrderIdValueConverter : ValueConverter<OrderId, Guid>
    {
        public OrderIdValueConverter() : base
            (
                id => id.Value,
                value => new OrderId(value)
            )
        { }
    }
}
