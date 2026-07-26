using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFramwork
{
    public sealed class UnitOfWork<TDbContext>(TDbContext dbContext) : IUnitOfWork
        where TDbContext : DbContext
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
