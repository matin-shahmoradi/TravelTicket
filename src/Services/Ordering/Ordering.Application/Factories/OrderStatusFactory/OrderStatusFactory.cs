using BuildingBlocks.Constants;
using Ordering.Domain.Enums;

namespace Ordering.Application.Factories.OrderStatusFactory
{
    public sealed class OrderStatusFactory : IOrderStatusFactory
    {
        private IReadOnlyDictionary<string, OrderStatus> StatusMap =
            new Dictionary<string, OrderStatus>(StringComparer.OrdinalIgnoreCase)
            {
                [PaymentStatuses.Succeeded] = OrderStatus.Completed,
                [PaymentStatuses.Failed] = OrderStatus.Cancelled,
                [PaymentStatuses.Pending] = OrderStatus.Pending,
            };
        public OrderStatus CreateFromPaymentStatus(string paymentStatus)
        {
            if (string.IsNullOrEmpty(paymentStatus))
                throw new ArgumentNullException(nameof(paymentStatus));

            if (StatusMap.TryGetValue(paymentStatus, out var orderStatus))
                throw new InvalidOperationException($"Unsupported payment status: {paymentStatus}");

            return orderStatus;
        }
    }
}
