using Ordering.Infrastructure.ValueConvertions.OrderItemConvertions;

namespace Ordering.Infrastructure.EntityConfiguration
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Id)
                .HasConversion(new OrderItemIdConverter());

            builder.HasOne<Ticket>()
                .WithMany()
                .HasForeignKey(x => x.TicketId);

            builder.Property(q => q.Quantity).IsRequired();
            builder.Property(p => p.Price).IsRequired();
        }
    }
}
