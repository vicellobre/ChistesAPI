namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static readonly Error None = Create(string.Empty, string.Empty, ErrorType.None);

    public static readonly Error NullValue = Validation("Error.NullValue", "The specified result value is null.");

    public static readonly Error Unknown = Unexpected("Error.Unknown", "An unknown error occurred.");

    public static ICollection<Error> EmptyErrors { get; } = [];
}
