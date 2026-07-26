using Ordering.Domain.Enums;

namespace Ordering.Application.Factories.OrderStatusFactory
{
    public interface IOrderStatusFactory
    {
        OrderStatus CreateFromPaymentStatus(string paymentStatus);
    }
}
