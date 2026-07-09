using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.api.Data.ValueConverters;
using Payment.api.Domain;

namespace Payment.api.Data
{
    public class PaymentModelConfiguration : IEntityTypeConfiguration<PaymentModel>
    {
        public void Configure(EntityTypeBuilder<PaymentModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(new PaymentIdValueConverter());

            builder.Property(x => x.OrderId)
                .HasConversion(new OrderIdValueConverter())
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .HasConversion(new CustomerIdValueConverter())
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Authority)
                .HasMaxLength(100);

            builder.Property(x => x.TransactionId)
                .HasMaxLength(100);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.PaidAtUtc);

            builder.Property(x => x.Version)
                .IsConcurrencyToken();

            builder.HasIndex(x => x.OrderId);
        }
    }
}
