namespace Catalog.API.Tickets.GetTicketsById
{
    public record GetTicketByIdQuery(Guid Id) : IQuery<Result<Ticket>>;
    internal sealed class GetTicketByIdQueryHandler(IDocumentSession session)
        : IQueryHandler<GetTicketByIdQuery, Result<Ticket>>
    {
        public async Task<Result<Ticket>> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            var ticket = await session.LoadAsync<Ticket>(query.Id);
            if(ticket is null)
            {
                return Result<Ticket>.Failure(Error.CustomError("Ticket Not found!",404,ErrorType.NOT_FOUND));
            }
            return Result<Ticket>.Success(ticket);
        }
    }
}
