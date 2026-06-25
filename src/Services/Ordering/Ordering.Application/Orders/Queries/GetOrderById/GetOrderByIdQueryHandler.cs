using Microsoft.EntityFrameworkCore;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Queries.GetOrderById
{
    internal sealed class GetOrderByIdQueryHandler(IOrderDbContext orderDbContext)
        : IQueryHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await orderDbContext.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.Id == OrderId.Of(query.OrderId))
                .Join(
                    inner: orderDbContext.Customers,
                    o => o.CustomerId,
                    c => c.Id,
                    (order, customer) => new { order, customer }
                )
                .FirstOrDefaultAsync(cancellationToken);


            if (order is null)
                return Result<OrderDto>.Failure(Error.NotFoundError("Order Not Found!"));

            var dto = new OrderDto
            (
                Id: order.order.Id.Value,
                Customer: new CustomerDto(
                    CustomerId: order.customer!.Id.Value,
                    nationalCode: "",
                    Name: order.customer.Email,
                    Email: order.customer.Email,
                    PhoneNumber: order.customer.PhoneNumber),
                OrderStatus: order.order.OrderStatus,
                OrderItems: order.order.OrderItems.Select(x => new OrderItemDto(
                    OrderId: x.OrderId.Value,
                    TicketId: x.TicketId.Value,
                    Quantity: x.Quantity,
                    Price: x.Price
                    )).ToList()
            );

            return Result<OrderDto>.Success(dto);
        }
    }
}
