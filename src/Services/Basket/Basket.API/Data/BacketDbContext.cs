using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Basket.API.Data
{
    public class BacketDbContext : DbContext
    {
        public BacketDbContext(DbContextOptions<BacketDbContext> options) : base(options)
        {
            
        }
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
