using Basket.API.Data.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.API.Data.EntityConfigurations
{
    public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(new ShoppingCartItemIdConverter())
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TicketId).IsRequired();

            builder.Property(x => x.Quantity).IsRequired();

            builder.Property(x => x.Price).IsRequired();
        }
    }
}
