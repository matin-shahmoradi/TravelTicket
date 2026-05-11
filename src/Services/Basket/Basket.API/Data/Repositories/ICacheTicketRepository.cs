namespace Basket.API.Data.Repositories
{
    public interface ICacheTicketRepository
    {
        Task<string> ReadTicketFromCacheAsync(string ticketId , CancellationToken cancellationToken);
        Task StoreTicketInCacheAsync(TicketReadModel ticket, CancellationToken cancellationToken);
    }
}
