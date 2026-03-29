using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Persistence.Constants;
using Enjoy.Persistence.Users.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjoy.Persistence.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasConversion(new NameConverter())
            .HasMaxLength(Name.MaxLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(new EmailConverter())
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasConversion(new PasswordHashConverter())
            .HasMaxLength(PasswordHash.MaxLength)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion(new RoleConverter())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.IdentityId)
            .IsRequired();

        builder.Property(u => u.CreatedOnUtc)
            .IsRequired();

        builder.Property(u => u.ModifiedOnUtc);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.IdentityId)
            .IsUnique();
    }
}
