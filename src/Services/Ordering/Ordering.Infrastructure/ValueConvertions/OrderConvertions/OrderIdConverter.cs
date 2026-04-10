using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Infrastructure.EFCoreConvertion.OrderConvertions
{
    internal sealed class OrderIdConverter : ValueConverter<OrderId,Guid>
    {
        public OrderIdConverter() 
            : base(
                  orderId => orderId.Value ,
                  value => new OrderId(value)
                  ) 
        { }
    }
}
