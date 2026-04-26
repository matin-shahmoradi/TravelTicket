using Catalog.API.Domain.ValueObjects;

namespace Catalog.API.Tickets.GetTicketsById
{
    public record GetTicketByIdQuery(Guid Id) : IQuery<Result<Ticket>>;
    internal sealed class GetTicketByIdQueryHandler(ICatalogDbContext CatalogDb)
        : IQueryHandler<GetTicketByIdQuery, Result<Ticket>>
    {
        public async Task<Result<Ticket>> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            var ticket = await CatalogDb.Tickets.FindAsync(TicketId.New(query.Id),cancellationToken);
            if(ticket is null)
            {
                return Result<Ticket>.Failure(Error.CustomError("Ticket Not found!",404,ErrorType.NOT_FOUND));
            }
            return Result<Ticket>.Success(ticket);
        }
    }
}
