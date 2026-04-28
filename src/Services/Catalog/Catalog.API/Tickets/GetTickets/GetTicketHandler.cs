using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Tickets.GetTickets
{
    public record GetTicketQuery(GetProductRequest Request) : IQuery<Result<IEnumerable<Ticket>>>;

    internal sealed class GetTicketQueryHandler(ICatalogDbContext catalogDb)
        : IQueryHandler<GetTicketQuery, Result<IEnumerable<Ticket>>>
    {
        public async Task<Result<IEnumerable<Ticket>>> Handle(GetTicketQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Request.PageNumber;
            var pageSize = request.Request.PageSize;

            var tickets = await catalogDb.Tickets
                .AsNoTracking()
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.TravelDate)
                .ToListAsync(cancellationToken);
                

            if (tickets is null) 
            {
                return Result<IEnumerable<Ticket>>.Failure(
                    Error.NotFoundError(message:"Ticket Not Found!")); 
            }
            return Result<IEnumerable<Ticket>>.Success(tickets);
        }
    }
}
    