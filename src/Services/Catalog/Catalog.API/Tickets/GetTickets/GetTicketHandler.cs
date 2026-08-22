using BuildingBlocks.Pagination;
using Catalog.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Tickets.GetTickets
{
    public record GetTicketQuery(int PageNumber = 1, int PageSize = 10) : IQuery<Result<PagedResult<TicketDto>>>;
    internal sealed class GetTicketQueryHandler(ICatalogDbContext catalogDb)
        : IQueryHandler<GetTicketQuery, Result<PagedResult<TicketDto>>>
    {
        public async Task<Result<PagedResult<TicketDto>>> Handle(GetTicketQuery request, CancellationToken cancellationToken)
        {
            var pageRequest = new PageRequest
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };

            var ticketQuery = catalogDb.Tickets
                .AsNoTracking()
                .OrderByDescending(x => x.TravelDate)
                .ThenByDescending(x => x.Id);

            var pagedResult = await ticketQuery.ToPagedResultAsync(
                pageRequest: pageRequest,
                projection: source => source.Select(ticket => new TicketDto
                (
                    Id: ticket.Id.Value,
                    Origin: ticket.Origin,
                    Destination: ticket.Destination,
                    Description: ticket.Description,
                    TravelDate: ticket.TravelDate,
                    Price: ticket.Price
                )), cancellationToken);

            if (pagedResult is not null)
                return Result<PagedResult<TicketDto>>.Success(pagedResult);

            return Result<PagedResult<TicketDto>>.Failure(Error.NotFoundError("ticket not found"));
        }
    }
}
