using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data.Repositories
{
    public class CacheTicketRepository(IDistributedCache cache) : ICacheTicketRepository
    {
        public async Task<string> ReadTicketFromCacheAsync(string ticketId, CancellationToken cancellationToken)
        {
            var getCachedTicket = await cache.GetStringAsync(ticketId, cancellationToken);
            return getCachedTicket!;
        }
        
        public async Task StoreTicketInCacheAsync(TicketReadModel ticket, CancellationToken cancellationToken)
        {
            await cache.SetStringAsync(
               key: ticket.TicketId.ToString(),
               value: JsonSerializer.Serialize(ticket));
        }
    }
}
