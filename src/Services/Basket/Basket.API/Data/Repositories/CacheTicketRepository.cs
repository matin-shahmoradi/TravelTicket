using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data.Repositories
{
    public class CacheTicketRepository(IDistributedCache cache) : ICacheTicketRepository
    {
        public async Task<TicketReadModel> ReadTicketFromCacheAsync(string ticketId, CancellationToken cancellationToken)
        {
            var getCachedTicket = await cache.GetStringAsync(ticketId, cancellationToken);
            if (string.IsNullOrEmpty(getCachedTicket))
            {
                // TO DO : Get data from catalog using REST api.
            }

            var cachedTicket = JsonSerializer.Deserialize<TicketReadModel>(getCachedTicket!);
            return cachedTicket!;
        }

        public async Task StoreTicketInCacheAsync(TicketReadModel ticket, CancellationToken cancellationToken)
        {
            var getCachedTicket = await cache.GetStringAsync(ticket.TicketId.ToString(), cancellationToken);
            if (!string.IsNullOrEmpty(getCachedTicket))
            {
                await cache.SetStringAsync(
                   key: ticket.TicketId.ToString(),
                   value: JsonSerializer.Serialize(ticket));   
            }
        }
    }
}
