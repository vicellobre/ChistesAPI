using Enjoy.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Users.ValueConverters;

public sealed class RoleConverter : ValueConverter<Role, string>
{
    public RoleConverter()
        : base(
            role => role.Value,
            value => Role.Create(value).Value)
    {
    }
}
