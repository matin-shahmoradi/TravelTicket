using Microsoft.EntityFrameworkCore;
using Ordering.Application.Extensions;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Queries.GetOrdersByPhoneNumber
{
    internal sealed class GetOrdersByCustomerIdQueryHandler(IOrderDbContext orderContext) :
        IQueryHandler<GetOrdersByCustomerIdQuery, Result<GetOrderByCustomerIdResult>>
    {
        public async Task<Result<GetOrderByCustomerIdResult>> Handle(GetOrdersByCustomerIdQuery query, CancellationToken cancellationToken)
        {
            var orders = await orderContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == CustomerId.New(query.CustomerId))
                .OrderBy(o => o.OrderName.Value)
                .ToListAsync(cancellationToken);

            if (!orders.Any())
            {
                return Result<GetOrderByCustomerIdResult>.Failure(Error.NotFoundError());
            }
            return Result<GetOrderByCustomerIdResult>.Success(GetOrderByCustomerIdResult.New(orders.ToOrderDtoList()));
        }
    }
}
