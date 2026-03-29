using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.Jokes.ValueObjects;

public readonly record struct JokeText
{
    public string Value { get; }

    public const int MaxLength = 2000;

    private JokeText(string value) => Value = value;

    public static Result<JokeText> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<JokeText>.Failure(Error.JokeText.IsNullOrEmpty);

        var normalized = value.NormalizeWhitespace();
        if (normalized.Length > MaxLength)
            return Result<JokeText>.Failure(Error.JokeText.TooLong(MaxLength));

        return Result<JokeText>.Success(new JokeText(normalized));
    }

    public override string ToString() => Value;

    public static implicit operator string(JokeText text) => text.Value;
}