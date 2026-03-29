using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Messaging;
using Enjoy.Domain.Shared.Primitives;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.Events;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Domain.Users.Entities;

public sealed class User : AggregateRoot, IAuditableEntity
{
    public static string NewId() => $"usr_{Guid.CreateVersion7()}";

    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public Role Role { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public Guid IdentityId { get; set; }

    private User() : base() { }

    private User(string id, Name name, Email email, PasswordHash passwordHash, Role role) : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public static Result<User> Create(string name, string email, string passwordHash, string role)
    {
        List<Error> errors = [];

        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            errors.AddRange(nameResult.Errors);

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            errors.AddRange(emailResult.Errors);

        var passwordHashResult = PasswordHash.Create(passwordHash);
        if (passwordHashResult.IsFailure)
            errors.AddRange(passwordHashResult.Errors);

        var roleResult = Role.Create(role);
        if (roleResult.IsFailure)
            errors.AddRange(roleResult.Errors);

        if (!errors.IsEmpty())
            return Result<User>.Failure(errors);

        User user = new(
            NewId(),
            nameResult.Value,
            emailResult.Value,
            passwordHashResult.Value,
            roleResult.Value);
        user.RaiseDomainEvent(new UserRegisteredDomainEvent(DomainEvent.NewId(), user.Id));
        return Result<User>.Success(user);
    }

    public Result ChangeName(string name)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure(nameResult.Errors);
        if (Name == nameResult.Value)
            return Result.Success();

        Name = nameResult.Value;
        RaiseDomainEvent(new UserNameChangedDomainEvent(DomainEvent.NewId(), Id));
        return Result.Success();
    }

    public Result ChangeEmail(string email)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Errors);
        if (Email == emailResult.Value)
            return Result.Success();

        Email = emailResult.Value;
        RaiseDomainEvent(new UserEmailChangedDomainEvent(DomainEvent.NewId(), Id));
        return Result.Success();
    }

    public Result SetPasswordHash(string passwordHash)
    {
        var result = PasswordHash.Create(passwordHash);
        if (result.IsFailure)
            return Result.Failure(result.Errors);
        if (PasswordHash.Value == result.Value.Value)
            return Result.Success();

        PasswordHash = result.Value;
        RaiseDomainEvent(new UserPasswordChangedDomainEvent(DomainEvent.NewId(), Id));
        return Result.Success();
    }

    public Result ChangeRole(string role)
    {
        var roleResult = Role.Create(role);
        if (roleResult.IsFailure)
            return Result.Failure(roleResult.Errors);
        if (Role == roleResult.Value)
            return Result.Success();

        Role = roleResult.Value;
        RaiseDomainEvent(new UserRoleChangedDomainEvent(DomainEvent.NewId(), Id, Role.Value));
        return Result.Success();
    }
}
