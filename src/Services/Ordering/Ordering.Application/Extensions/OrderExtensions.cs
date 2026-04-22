namespace Ordering.Application.Extensions
{
    public static class OrderExtensions
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto(
                Id: order.Id.Value,
                CustomerId: order.CustomerId.Value,
                OrderName: order.OrderName.Value,
                Payment: new PaymentDto(
                    CardName: order.Payment.CardName!,
                    CardNumber: order.Payment.CardNumber,
                    Expiration: order.Payment.Expiration,
                    CVV: order.Payment.CVV,
                    PaymentMethod: order.Payment.PaymentMethod),
                OrderStatus: order.OrderStatus,
                OrderItems: order.OrderItems.Select(oi => new OrderItemDto(
                    OrderId: oi.OrderId.Value,
                    TicketId: oi.TicketId.Value,
                    Quantity: oi.Quantity,
                    Price: oi.Price))
                .ToList()
                );
        }
        public static IEnumerable<OrderDto> ToOrderDtoList(this IEnumerable<Order> orders)
        {
            return orders.Select(order => new OrderDto(
                Id: order.Id.Value,
                CustomerId: order.CustomerId.Value,
                OrderName: order.OrderName.Value,
                Payment: new PaymentDto(
                    CardName: order.Payment.CardName!,
                    CardNumber: order.Payment.CardNumber,
                    Expiration: order.Payment.Expiration,
                    CVV: order.Payment.CVV,
                    PaymentMethod: order.Payment.PaymentMethod
                    ),
                OrderStatus: order.OrderStatus,
                OrderItems: order.OrderItems.Select(oi => new OrderItemDto(
                    OrderId: oi.OrderId.Value,
                    TicketId: oi.TicketId.Value,
                    Quantity: oi.Quantity,
                    Price: oi.Price))
                .ToList()
                ));
        }
    }
}
