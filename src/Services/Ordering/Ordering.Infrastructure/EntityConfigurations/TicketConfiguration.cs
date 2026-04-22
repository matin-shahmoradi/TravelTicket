using Ordering.Infrastructure.ValueConvertions.TicketConvertions;

namespace Ordering.Infrastructure.EntityConfigurations
{
    internal class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(new TicketIdConverter());

            builder.Property(x => x.Origin)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Destination)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Price)
                .IsRequired();

            builder.Property(x => x.SeatNumber)
                .IsRequired();
        }
    }
}
