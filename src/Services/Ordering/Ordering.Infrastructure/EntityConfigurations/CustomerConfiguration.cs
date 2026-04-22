using Ordering.Infrastructure.EFCoreConvertions.CustomerConvertions;

namespace Ordering.Infrastructure.EntityConfiguration
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(new CustomerIdConverter());

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();       

            builder.Property(x => x.NationalCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(255);

            builder.HasIndex(x => x.PhoneNumber);
            builder.HasIndex(x => x.NationalCode);
        }
    }
}
