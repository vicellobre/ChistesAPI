namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class Email
    {
        public static readonly Error IsNullOrEmpty = Validation("Email.IsNullOrEmpty", "The email cannot be empty.");

        public static readonly Error InvalidFormat = Validation("Email.InvalidFormat", "The email does not meet the formatting guidelines.");

        public static readonly Func<int, Error> TooLong = length => Validation("Email.TooLong", $"The email cannot exceed {length} characters.");

        public static readonly Func<int, Error> TooShort = length => Validation("Email.TooShort", $"The email must be at least {length} characters.");
    }
}
