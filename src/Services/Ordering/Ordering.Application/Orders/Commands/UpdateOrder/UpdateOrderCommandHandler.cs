using Ordering.Application.Factories.OrderStatusFactory;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    internal sealed class UpdateOrderCommandHandler(
        IOrderDbContext orderContext,
        IOrderStatusFactory orderStatusFactory) :
        ICommandHandler<UpdateOrderCommand, Result<UpdateOrderStatusDto>>
    {
        public async Task<Result<UpdateOrderStatusDto>> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = command.UpdateOrderDto.OrderId;

            var order = await orderContext.Orders.FindAsync(orderId, cancellationToken);

            if (order is null)
            {
                return Result<UpdateOrderStatusDto>.Failure(
                    Error.NotFoundError(message: $"Order with Id {orderId} Not Found!"));
            }

            var orderStatus = orderStatusFactory.CreateFromPaymentStatus(command.UpdateOrderDto.PaymentStatus);

            order.UpdateOrderStatus(orderStatus);

            await orderContext.SaveChangesAsync(cancellationToken);

            var result = new UpdateOrderStatusDto(
                command.UpdateOrderDto.OrderId,
                command.UpdateOrderDto.PaymentStatus);

            return Result<UpdateOrderStatusDto>.Success(result);
        }
    }
}
