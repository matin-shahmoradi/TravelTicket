using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Infrastructure.EFCoreConvertions.CustomerConvertions
{
    internal sealed class CustomerIdConverter : ValueConverter<CustomerId , Guid>
    {
        public CustomerIdConverter() : 
            base(
                customerId => customerId.Value,
                value => CustomerId.New(value))
        {

        }
    }
}
