namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class Name
    {
        public static readonly Error IsNullOrEmpty = Validation("FirstName.IsNullOrEmpty", "The first name cannot be empty.");

        public static readonly Error InvalidFormat = Validation("FirstName.InvalidFormat", "First name format is invalid.");

        public static readonly Func<int, Error> TooLong = length => Validation("FirstName.TooLong", $"First name cannot be longer than {length} characters.");

        public static readonly Func<int, Error> TooShort = length => Validation("FirstName.TooShort", $"First name must be at least {length} characters.");
    }
}
