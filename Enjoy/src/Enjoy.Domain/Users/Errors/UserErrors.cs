namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public static class User
    {
        public static readonly Error IsNull = Validation("User.IsNull", "The user cannot be null.");

        public static readonly Error EmailAlreadyInUse = Conflict("User.EmailAlreadyInUse", "The specified email is already in use.");

        public static readonly Func<Guid, Error> NotFound = id => Error.NotFound("User.NotFound", $"The user with the identifier {id} was not found.");

        public static readonly Func<string, Error> EmailNotExist = email => Error.NotFound("User.EmailNotExist", $"The email {email} does not exist.");

        public static readonly Error InvalidCredentials = Unauthorized("User.InvalidCredentials", "The specified credentials are invalid.");

        public static readonly Error NoUsersFound = Error.NotFound("User.NoUsersFound", "No users were found.");
    }
}
