namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class PasswordHash
    {
        public static readonly Error IsNullOrEmpty = Validation("PasswordHash.IsNullOrEmpty", "The password hash cannot be empty.");

        public static readonly Func<int, Error> TooLong = length => Validation("PasswordHash.TooLong", $"The password hash cannot exceed {length} characters.");

        public static readonly Func<int, Error> TooShort = length => Validation("PasswordHash.TooShort", $"The password hash must be at least {length} characters.");
    }
}
