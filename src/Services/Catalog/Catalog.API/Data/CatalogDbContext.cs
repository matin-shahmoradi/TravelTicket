using BuildingBlocks.Abstractions;
using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Catalog.API.Data
{
    public class CatalogDbContext : DbContext, ICatalogDbContext, IUnitOfWork
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {

        }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfiguration(new OutboxMessageBaseEntityConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
