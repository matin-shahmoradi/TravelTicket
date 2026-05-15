using Basket.API.Data.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.API.Data.EntityConfigurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(new ShoppingCartIdConverter());

            builder.Property(x => x.Username)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasIndex(x => x.Username)
                .IsUnique();

            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(si => si.ShoppingCartId);
        }
    }
}
