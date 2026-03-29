using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Domain.Shared.Results;
using Enjoy.Infrastructure.Auth.Options;
using Enjoy.Persistence.Contexts;
using Enjoy.Persistence.Identity;
using Microsoft.Extensions.Options;

namespace Enjoy.Infrastructure.Auth;

internal sealed class AuthResultIssuer(
    ITokenProvider tokenProvider,
    ApplicationIdentityDbContext identityDbContext,
    IOptions<JwtOptions> options) : IAuthResultIssuer
{
    private readonly JwtOptions _options = options.Value;

    public async Task<Result<AuthResult>> IssueAsync(
        string userId,
        string email,
        IEnumerable<string> roles,
        Guid identityUserId,
        CancellationToken cancellationToken = default)
    {
        var tokens = tokenProvider.Create(userId, email, roles);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUserId.ToString(),
            Token = tokens.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays)
        };
        identityDbContext.RefreshTokens.Add(refreshToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResult(userId, tokens.AccessToken, tokens.RefreshToken));
    }
}
