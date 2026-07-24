using Basket.API.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFramwork
{
    public sealed class UnitOfWork<TContext>(TContext context) : IUnitOfWork
        where TContext : DbContext
    {
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
