namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class Role
    {
        public static readonly Error IsNullOrEmpty = Validation("Role.IsNullOrEmpty", "The role cannot be empty.");

        public static readonly Error Invalid = Validation("Role.Invalid", "The specified role is not valid. Valid values: User, Admin.");
    }
}
