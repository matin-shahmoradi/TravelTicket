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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
