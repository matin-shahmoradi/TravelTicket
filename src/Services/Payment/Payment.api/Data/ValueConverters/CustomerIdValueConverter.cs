using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payment.api.Domain.StrongIdTypes;

namespace Payment.api.Data.ValueConverters
{
    public sealed class CustomerIdValueConverter : ValueConverter<CustomerId, Guid>
    {
        public CustomerIdValueConverter() : base
            (
                id => id.Value,
                value => new CustomerId(value)
            )
        { }
    }
}
