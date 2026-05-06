namespace Basket.API.Data.Repositories
{
    public interface ICacheTicketRepository
    {
        Task<TicketReadModel> ReadTicketFromCacheAsync(string ticketId , CancellationToken cancellationToken);
        Task StoreTicketInCacheAsync(TicketReadModel ticket, CancellationToken cancellationToken);
    }
}
