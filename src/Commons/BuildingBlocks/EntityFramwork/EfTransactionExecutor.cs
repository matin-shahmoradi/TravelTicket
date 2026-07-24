using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BuildingBlocks.EntityFramwork
{
    public sealed class EfTransactionExecutor<TDbContext>(TDbContext dbContext) : ITransactionExecutor
        where TDbContext : DbContext
    {
        public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                if (dbContext.Database.CurrentTransaction != null)
                {
                    var result = await action(cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    return result;
                }
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var result = await action(cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}
