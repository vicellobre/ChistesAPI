using Enjoy.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Enjoy.Infrastructure.Auth;

internal sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<IdentityUser> _inner = new();

    public string Hash(string password) =>
        _inner.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash)
    {
        var result = _inner.VerifyHashedPassword(null!, passwordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}
