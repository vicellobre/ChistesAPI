using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.ValueObjects;
using Enjoy.Domain.Topics.Entities;
using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Jokes.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjoy.Persistence.Jokes;

public sealed class JokeConfiguration : IEntityTypeConfiguration<Joke>
{
    public void Configure(EntityTypeBuilder<Joke> builder)
    {
        builder.ToTable(TableNames.Jokes);

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Text)
            .HasConversion(new JokeTextConverter())
            .HasMaxLength(JokeText.MaxLength)
            .IsRequired();

        builder.Property(j => j.AuthorId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(j => j.Origin)
            .HasConversion(new OriginConverter())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(j => j.CreatedOnUtc)
            .IsRequired();

        builder.Property(j => j.ModifiedOnUtc);

        builder
            .HasMany(j => j.Topics)
            .WithMany(t => t.Jokes)
            .UsingEntity<Dictionary<string, object>>(
                "joke_topics",
                j => j.HasOne<Topic>().WithMany().HasForeignKey("TopicId"),
                t => t.HasOne<Joke>().WithMany().HasForeignKey("JokeId"));
    }
}
