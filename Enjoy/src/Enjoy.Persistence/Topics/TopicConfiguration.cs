using Enjoy.Domain.Topics.Entities;
using Enjoy.Domain.Topics.ValueObjects;
using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Topics.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjoy.Persistence.Topics;

public sealed class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable(TableNames.Topics);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasConversion(new TopicNameConverter())
            .HasMaxLength(TopicName.MaxLength)
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}
