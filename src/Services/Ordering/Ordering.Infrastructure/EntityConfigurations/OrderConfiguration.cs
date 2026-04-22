using Ordering.Domain.Enums;
using Ordering.Infrastructure.EFCoreConvertion.OrderConvertions;
using Ordering.Infrastructure.ValueConvertions.OrderConvertions;

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

            builder.Property(x => x.OrderName)
                .HasConversion(new OrderNameConverter())
                .HasColumnName(nameof(Order.OrderName))
                .HasMaxLength(200)
                .IsRequired();

            // Relations Configuration.
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

            // Complex Property Configurations.
            //builder.ComplexProperty(x => x.OrderName, buildName =>
            //{
            //    buildName
            //        .Property(n => n.Value)
            //        .HasColumnName(nameof(Order.OrderName))
            //        .HasMaxLength(100)
            //        .IsRequired();
            //});

            builder.ComplexProperty(p => p.Payment, buildPayment =>
            {
                buildPayment
                    .Property(x => x.CardName)
                    .HasMaxLength(150);

                buildPayment
                    .Property(x => x.CardNumber)
                    .HasMaxLength(16)
                    .IsRequired();

                buildPayment
                    .Property(x => x.CVV)
                    .HasMaxLength(5)
                    .IsRequired();

                buildPayment
                    .Property(x => x.Expiration)
                    .IsRequired();

                buildPayment
                    .Property(x => x.PaymentMethod)
                    .IsRequired();
            });


        }
    }
}
