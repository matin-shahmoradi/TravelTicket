using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.EntityFrameworkCore;
using Payment.api.Domain;
using System.Reflection;

namespace Payment.api.Data
{
    public sealed class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        { }

        public DbSet<PaymentModel> Payments => Set<PaymentModel>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OutboxMessageBaseEntityConfiguration());
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
