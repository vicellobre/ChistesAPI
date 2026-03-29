using Enjoy.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Users.ValueConverters;

public sealed class PasswordHashConverter : ValueConverter<PasswordHash, string>
{
    public PasswordHashConverter()
        : base(
            passwordHash => passwordHash.Value,
            value => PasswordHash.Create(value).Value)
    {
    }
}
