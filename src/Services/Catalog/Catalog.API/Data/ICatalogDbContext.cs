using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public interface ICatalogDbContext
    {
        DbSet<Ticket> Tickets { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellation);
    }
}
