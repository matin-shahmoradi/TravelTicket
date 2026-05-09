namespace Basket.API.Basket.GetItemFromCache
{
    public record GetItemFromCacheQuery(Guid TicketId) : IQuery<Result<TicketReadModel>>;

}
