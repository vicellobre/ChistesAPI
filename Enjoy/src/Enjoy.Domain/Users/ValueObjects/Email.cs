using System.Text.RegularExpressions;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.Users.ValueObjects;

public readonly record struct Email
{
    public string Value { get; }

    public const int MinLength = 8;
    public const int MaxLength = 50;

    public const string EmailPattern = @"^[^@]+@[^@]+$";

    private Email(string value)  => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure(Error.Email.IsNullOrEmpty);

        var normalized = value.Trim().ToLowerInvariant();
        List<Error> errors = [];
        if (normalized.Length < MinLength)
            errors.Add(Error.Email.TooShort(MinLength));
        if (normalized.Length > MaxLength)
            errors.Add(Error.Email.TooLong(MaxLength));
        if (!Regex.IsMatch(normalized, EmailPattern))
            errors.Add(Error.Email.InvalidFormat);

        return errors.IsEmpty()
            ? Result<Email>.Success(new Email(normalized))
            : Result<Email>.Failure(errors);
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
