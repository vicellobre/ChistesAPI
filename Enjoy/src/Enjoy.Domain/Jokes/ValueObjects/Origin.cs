using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.Jokes.ValueObjects;

public readonly record struct Origin
{
    public const string ChuckNorris = nameof(ChuckNorris);
    public const string DadJoke = nameof(DadJoke);
    public const string Local = nameof(Local);

    public static readonly IReadOnlyCollection<string> All = [ChuckNorris, DadJoke, Local];

    private static readonly HashSet<string> ValidValues =
        new(All, StringComparer.OrdinalIgnoreCase);

    public string Value { get; }

    private Origin(string value) => Value = value;

    public static Result<Origin> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Origin>.Failure(Error.Origin.IsNullOrEmpty);

        if (!ValidValues.TryGetValue(value.Trim(), out var canonical))
            return Result<Origin>.Failure(Error.Origin.Invalid);

        return Result<Origin>.Success(new Origin(canonical));
    }

    public static Origin FromChuckNorris() => new(ChuckNorris);
    public static Origin FromDadJoke() => new(DadJoke);
    public static Origin FromLocal() => new(Local);

    public override string ToString() => Value;

    public static implicit operator string(Origin origin) => origin.Value;
}
