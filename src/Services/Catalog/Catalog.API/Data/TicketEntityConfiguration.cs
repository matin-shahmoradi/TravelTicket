using Catalog.API.Data.Converters;
using Catalog.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Data
{
    public class TicketEntityConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasConversion(new TicketIdConverter());

            builder.Property(t => t.Origin)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Destination)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.TravelDate)
                .IsRequired();

            builder.Property(t => t.Price)
                .IsRequired();
        }
    }
}
