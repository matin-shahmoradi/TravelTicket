namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    internal sealed class UpdateOrderCommandHandler(IOrderDbContext orderContext) :
        ICommandHandler<UpdateOrderCommand, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = command.OrderDto.Id;
            var order = await orderContext.Orders.FindAsync(orderId, cancellationToken);

            if (order is null)
            {
                return Result<OrderDto>.Failure(
                    Error.NotFoundError(message: $"Order with Id {orderId} Not Found!"));
            }

            UpdateOrderWithNewValues(order, command.OrderDto);

            orderContext.Orders.Update(order);
            await orderContext.SaveChangesAsync(cancellationToken);

            return Result<OrderDto>.Success(command.OrderDto);
        }

        private void UpdateOrderWithNewValues(Order order, OrderDto orderDto)
        {
            var updatedPayment = Payment.New(
                cardName: orderDto.Payment.CardName,
                cardNumber: orderDto.Payment.CardNumber,
                expiration: orderDto.Payment.Expiration,
                cvv: orderDto.Payment.CVV,
                paymentMethod: orderDto.Payment.PaymentMethod);

            order.Update(
                orderName: OrderName.New(orderDto.OrderName),
                orderStatus: orderDto.OrderStatus,
                payment: updatedPayment);
        }
    }
}
