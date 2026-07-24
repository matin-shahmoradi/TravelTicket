using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public interface ICatalogDbContext
    {
        DbSet<Ticket> Tickets { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellation);
    }
}
