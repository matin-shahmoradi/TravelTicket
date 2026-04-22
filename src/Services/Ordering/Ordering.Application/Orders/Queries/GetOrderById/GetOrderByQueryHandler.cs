using Microsoft.EntityFrameworkCore;
using Ordering.Application.Extensions;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Queries.GetOrderById
{
    internal sealed class GetOrderByQueryHandler(IOrderDbContext OrderContext) :
        IQueryHandler<GetOrderByIdQuery, Result<GetOrderByIdQueryResult>>
    {
        public async Task<Result<GetOrderByIdQueryResult>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await OrderContext.Orders
                .Where(x => x.Id == OrderId.New(query.Id))
                .FirstOrDefaultAsync();

            if (order is null)
            {
                return Result<GetOrderByIdQueryResult>.Failure(Error.NotFoundError("Order not Found!"));
            }

            return Result<GetOrderByIdQueryResult>.Success(GetOrderByIdQueryResult.New(order.ToOrderDto()));
        }
    }
}
