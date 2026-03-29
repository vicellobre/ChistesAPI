using Enjoy.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Users.ValueConverters;

public sealed class NameConverter : ValueConverter<Name, string>
{
    public NameConverter()
        : base(
            name => name.Value,
            value => Name.Create(value).Value)
    {
    }
}
