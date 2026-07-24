using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastracture.Outbox
{
    public class OutboxMessageBaseEntityConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnName("content")
                .HasColumnType("JSONB")
                .IsRequired();

            builder.Property(x => x.OccuredOnUtc)
                 .IsRequired();

            builder.Property(x => x.ProcessedOnUtc);
            builder.Property(x => x.Error);
        }
    }
}
