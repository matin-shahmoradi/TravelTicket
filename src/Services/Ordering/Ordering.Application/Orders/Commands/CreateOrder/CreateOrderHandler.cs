using Microsoft.Extensions.Logging;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    internal sealed class CreateOrderHandler(
        IOrderDbContext orderContext,
        ILogger<CreateOrderHandler> logger)
        : ICommandHandler<CreateOrderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = CreateNewOrder(command.OrderDto);

            orderContext.Orders.Add(order);
            await orderContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Order : {orderId} Created for Customer : {customerId}",
                order.Id, order.CustomerId);

            return Result<Guid>.Success(command.OrderDto.Id);
        }

        private Order CreateNewOrder(OrderDto orderDto)
        {
            var newOrder = Order.Create(
                id: OrderId.New(),
                customerId: CustomerId.New(orderDto.Id),
                orderName: OrderName.New(orderDto.OrderName),
                payment: Payment.New(
                    orderDto.Payment.CardName,
                    orderDto.Payment.CardNumber,
                    orderDto.Payment.Expiration,
                    orderDto.Payment.CVV,
                    orderDto.Payment.PaymentMethod));

            foreach(var orderItemDto in orderDto.OrderItems)
            {
                newOrder.Add(
                    ticketId: TicketId.New(orderItemDto.TicketId),
                    quantity: orderItemDto.Quantity,
                    price: orderItemDto.Price);
            }
            return newOrder;
        }
    }
}
