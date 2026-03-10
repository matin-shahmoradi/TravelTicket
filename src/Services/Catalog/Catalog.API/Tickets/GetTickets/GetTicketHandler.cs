using Marten.Pagination;

namespace Catalog.API.Tickets.GetTickets
{
    public record GetTicketQuery(GetProductRequest Request) : IQuery<Result<IEnumerable<Ticket>>>;

    internal sealed class GetTicketQueryHandler(IDocumentSession session)
        : IQueryHandler<GetTicketQuery, Result<IEnumerable<Ticket>>>
    {
        public async Task<Result<IEnumerable<Ticket>>> Handle(GetTicketQuery request, CancellationToken cancellationToken)
        {
            var products = await session
                .Query<Ticket>()    
                .ToPagedListAsync(
                request.Request.PageNumber ?? 1,
                request.Request.PageSize ?? 10);

            if (products is null) 
            { 
                return Result<IEnumerable<Ticket>>.Failure(
                    Error.CustomError(
                        mesaage:"Ticket Not Found" ,
                        code:400,
                        errorType:ErrorType.NOT_FOUND)); 
            }
            return Result<IEnumerable<Ticket>>.Success(products);
        }
    }
}
    