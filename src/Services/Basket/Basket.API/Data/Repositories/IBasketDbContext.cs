using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Basket.API.Data.Repositories
{
    public interface IBasketDbContext
    {
        public DbSet<OutboxMessage> outboxMessages { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
