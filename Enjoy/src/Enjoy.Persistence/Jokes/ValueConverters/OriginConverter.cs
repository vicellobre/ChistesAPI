using Enjoy.Domain.Jokes.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Jokes.ValueConverters;

public sealed class OriginConverter : ValueConverter<Origin, string>
{
    public OriginConverter()
        : base(
            origin => origin.Value,
            value => Origin.Create(value).Value)
    {
    }
}
