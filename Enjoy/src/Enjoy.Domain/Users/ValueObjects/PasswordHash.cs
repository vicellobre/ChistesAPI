using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.Users.ValueObjects;

public readonly record struct PasswordHash
{
    public string Value { get; }

    public const int MinLength = 8;
    public const int MaxLength = 500;

    private PasswordHash(string value) => Value = value;

    public static Result<PasswordHash> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<PasswordHash>.Failure(Error.PasswordHash.IsNullOrEmpty);

        var normalized = value.Trim();
        List<Error> errors = [];
        if (normalized.Length < MinLength)
            errors.Add(Error.PasswordHash.TooShort(MinLength));
        if (normalized.Length > MaxLength)
            errors.Add(Error.PasswordHash.TooLong(MaxLength));

        return errors.IsEmpty()
            ? Result<PasswordHash>.Success(new(normalized))
            : Result<PasswordHash>.Failure(errors);
    }

    public override string ToString() => Value;

    public static implicit operator string(PasswordHash hash) => hash.Value;
}
