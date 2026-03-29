using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Domain.Users.ValueObjects;

public readonly record struct Role
{
    public const string User = nameof(User);
    public const string Admin = nameof(Admin);

    public static readonly IReadOnlyCollection<string> All = [User, Admin];

    private static readonly HashSet<string> ValidValues =
        new(All, StringComparer.OrdinalIgnoreCase);

    public string Value { get; }

    private Role(string value) => Value = value;

    public static Result<Role> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Role>.Failure(Error.Role.IsNullOrEmpty);

        if (!ValidValues.TryGetValue(value.Trim(), out var canonical))
            return Result<Role>.Failure(Error.Role.Invalid);

        return Result<Role>.Success(new Role(canonical));
    }

    public static Role CreateUser() => new(User);
    public static Role CreateAdmin() => new(Admin);

    public override string ToString() => Value;

    public static implicit operator string(Role role) => role.Value;
}
