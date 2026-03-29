using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.Topics.ValueObjects;

public readonly record struct TopicName
{
    public string Value { get; }

    public const int MinLength = 2;
    public const int MaxLength = 100;

    private TopicName(string value) => Value = value;

    public static Result<TopicName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<TopicName>.Failure(Error.TopicName.IsNullOrEmpty);

        var normalized = value.Trim().CapitalizeWords();
        List<Error> errors = [];
        if (normalized.Length < MinLength)
            errors.Add(Error.TopicName.TooShort(MinLength));
        if (normalized.Length > MaxLength)
            errors.Add(Error.TopicName.TooLong(MaxLength));

        return errors.IsEmpty()
            ? Result<TopicName>.Success(new TopicName(normalized))
            : Result<TopicName>.Failure(errors);
    }

    public override string ToString() => Value;

    public static implicit operator string(TopicName name) => name.Value;
}
