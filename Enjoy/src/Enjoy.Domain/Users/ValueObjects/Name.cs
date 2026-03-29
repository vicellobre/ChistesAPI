using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Results;
using System.Text.RegularExpressions;

namespace Enjoy.Domain.Users.ValueObjects;

public readonly record struct Name
{
    public string Value { get; }

    public const int MinLength = 2;
    public const int MaxLength = 50;

    public const string Pattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑçÇüÜàÀèÈìÌòÒùÙâêÊîôûäëïöüß\s]+$";

    private Name(string value) => Value = value;

    public static Result<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Name>.Failure(Error.Name.IsNullOrEmpty);

        var normalized = value.Trim().CapitalizeWords();
        List<Error> errors = [];
        if (normalized.Length < MinLength)
            errors.Add(Error.Name.TooShort(MinLength));
        if (normalized.Length > MaxLength)
            errors.Add(Error.Name.TooLong(MaxLength));
        if (!Regex.IsMatch(normalized, Pattern))
            errors.Add(Error.Name.InvalidFormat);

        return errors.IsEmpty()
            ? Result<Name>.Success(new(normalized))
            : Result<Name>.Failure(errors);
    }

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.Value;
}
