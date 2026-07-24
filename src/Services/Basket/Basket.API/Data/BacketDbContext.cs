using Basket.API.Data.Repositories;
using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Basket.API.Data
{
    public class BacketDbContext : DbContext, IBasketDbContext
    {
        public BacketDbContext(DbContextOptions<BacketDbContext> options) : base(options)
        {

        }
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();
        public DbSet<OutboxMessage> outboxMessages => Set<OutboxMessage>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OutboxMessageBaseEntityConfiguration());
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
