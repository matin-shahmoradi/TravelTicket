using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payment.api.Domain.StrongIdTypes;

namespace Payment.api.Data.ValueConverters
{
    public sealed class PaymentIdValueConverter : ValueConverter<PaymentId, Guid>
    {
        public PaymentIdValueConverter() : base
             (
                id => id.Value,
                value => new PaymentId(value)
             )
        { }
    }
}
