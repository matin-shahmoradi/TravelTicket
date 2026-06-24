using Ordering.Infrastructure.ValueConvertions.OrderItemConvertions;
using Ordering.Infrastructure.ValueConvertions.TicketConvertions;

namespace Ordering.Infrastructure.EntityConfiguration
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(new OrderItemIdConverter());
            builder.Property(x => x.TicketId)
                .HasConversion(new TicketIdConverter());

            builder.Property(q => q.Quantity).IsRequired();
            builder.Property(p => p.Price).IsRequired();
        }
    }
}
