using Basket.API.Data.Repositories;

namespace Basket.API.Basket.GetItemFromCache
{
    public class GetItemFromCacheQueryHandler(ICacheTicketRepository cacheTicketRepository)
        : IQueryHandler<GetItemFromCacheQuery, Result<TicketReadModel>>
    {
        public async Task<Result<TicketReadModel>> Handle(GetItemFromCacheQuery query, CancellationToken cancellationToken)
        {
            var ticket = await cacheTicketRepository.ReadTicketFromCacheAsync(query.TicketId.ToString(), cancellationToken);

            if(ticket is null)
            {
                return Result<TicketReadModel>.Failure(Error.NotFoundError());
            }

            return Result<TicketReadModel>.Success(ticket);
        }
    }
}
