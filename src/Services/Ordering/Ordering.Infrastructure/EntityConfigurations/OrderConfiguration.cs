using Ordering.Domain.Enums;
using Ordering.Infrastructure.EFCoreConvertion.OrderConvertions;

namespace Ordering.Infrastructure.EntityConfiguration
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            // Convertion Configuration.
            builder.Property(x => x.Id)
                .HasConversion(new OrderIdConverter());

            // Relation Configurations.
            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId);

            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);

            // Property Configurations.
            builder.Property(o => o.OrderStatus)
                .HasDefaultValue(OrderStatus.Draft)
                .HasConversion(
                    s => s.ToString(),
                    dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus));

            builder.Property(x => x.TotalPrice)
                .IsRequired();
        }
    }
}
