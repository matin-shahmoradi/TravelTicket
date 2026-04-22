using Microsoft.EntityFrameworkCore;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrdersByPhoneNumber
{
    internal sealed class GetOrdersByPhoneNumberQueryHandler(IOrderDbContext OrderContext)
        : IQueryHandler<GetOrdersByPhoneNumberQuery, Result<GetOrdersByPhoneNumberResult>>
    {
        public async Task<Result<GetOrdersByPhoneNumberResult>> Handle(GetOrdersByPhoneNumberQuery query, CancellationToken cancellationToken)
        {
            var order = await OrderContext.Orders
                .AsNoTracking()
                .Include(c => c.Customer)
                .Where(o => o.Customer.PhoneNumber == query.phoneNumber)
                .ToListAsync(cancellationToken);

            if (order is null)
            {
                return Result<GetOrdersByPhoneNumberResult>
                    .Failure(Error.NotFoundError(message: "Order Not Found"));
            }
                
            return Result<GetOrdersByPhoneNumberResult>
                .Success(GetOrdersByPhoneNumberResult.New(order.ToOrderDtoList()));
        }
    }
}
