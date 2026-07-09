using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;

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
                await using var transaction = dbContext.Database.BeginTransaction();
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
