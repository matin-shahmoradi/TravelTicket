using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Catalog.API.Data
{
    public class CatalogDbContext : DbContext , ICatalogDbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {

        }

        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
