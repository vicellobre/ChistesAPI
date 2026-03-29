using Enjoy.Domain.Jokes.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Jokes.ValueConverters;

public sealed class JokeTextConverter : ValueConverter<JokeText, string>
{
    public JokeTextConverter()
        : base(
            jokeText => jokeText.Value,
            value => JokeText.Create(value).Value)
    {
    }
}
