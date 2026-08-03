using Catalog.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repository
{
    public class TicketQueryRepository(CatalogDbContext context) : ITicketQueryRepository
    {
        public async Task<Ticket?> GetTicketById(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Tickets.FindAsync(id, cancellationToken);
        }

        public async Task<Ticket?> GetTicketByIdWithNoTracking(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Tickets.SingleOrDefaultAsync(x => x.Id == TicketId.New(id), cancellationToken);
        }
    }
}
