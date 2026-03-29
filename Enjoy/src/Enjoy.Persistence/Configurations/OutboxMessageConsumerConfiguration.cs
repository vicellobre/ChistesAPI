using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjoy.Persistence.Configurations;

internal sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable(TableNames.OutboxMessageConsumers);

        builder.HasKey(c => new { c.Id, c.Name });

        builder.Property(c => c.Id)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(500)
            .IsRequired();
    }
}
