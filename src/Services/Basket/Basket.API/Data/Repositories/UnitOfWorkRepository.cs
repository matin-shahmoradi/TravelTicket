namespace Basket.API.Data.Repositories
{
    public sealed class UnitOfWorkRepository(BacketDbContext basketDbContext) : IUnitOfWork
    {
        public async Task SaveChangesAsync(CancellationToken cancellation) =>
            await basketDbContext.SaveChangesAsync(cancellation);
    }
}
