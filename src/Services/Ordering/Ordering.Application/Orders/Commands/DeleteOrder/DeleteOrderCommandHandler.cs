using Microsoft.Extensions.Logging;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{
    internal sealed class DeleteOrderCommandHandler(
        IOrderDbContext orderContext , 
        ILogger<DeleteOrderCommandHandler> logger)
        : ICommandHandler<DeleteOrderCommand, Result<OrderId>>
    {
        public async Task<Result<OrderId>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderContext.Orders.FindAsync(command.OrderId,cancellationToken);
            if (order is null)
            {
                return Result<OrderId>.Failure(
                    Error.NotFoundError(
                        message:$"Order with id {command.OrderId.Value} Not Found!"));
            }

            orderContext.Orders.Remove(order);
            await orderContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Order {OrderId} Deleted for Customer {CustomerId}",
                order.Id,order.CustomerId);

            return Result<OrderId>.Success(command.OrderId);
        }
    }
}
