using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrders
{
    internal sealed class GetOrdersQueryHandler(IOrderDbContext OrderContext)
        : IQueryHandler<GetOrdersQuery, Result<PagedResult<OrderDto>>>
    {
        public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            int pageIndex = query.PaginationRequest.pageIndex;
            int pageSize = query.PaginationRequest.pageSize;

            var totalCount = await OrderContext.Orders.LongCountAsync(cancellationToken);

            var orders = await OrderContext.Orders
                .AsNoTracking()
                .Include(oi => oi.OrderItems)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return Result<PagedResult<OrderDto>>
                .Success(new PagedResult<OrderDto>
                (pageIndex,pageSize,totalCount,orders.ToOrderDtoList()));

        }
    }
}
