using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Enjoy.Infrastructure.Auth;

internal sealed class TokenProvider(IOptions<JwtOptions> options) : ITokenProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public AccessTokenResponse Create(string userId, string email, IEnumerable<string> roles)
    {
        string accessToken = GenerateAccessToken(userId, email, roles);
        string refreshToken = GenerateRefreshToken();
        return new AccessTokenResponse(accessToken, refreshToken);
    }

    private string GenerateAccessToken(string userId, string email, IEnumerable<string> roles)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
        ];
        foreach (string role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };

        JsonWebTokenHandler handler = new();
        string accessToken = handler.CreateToken(descriptor);
        return accessToken;
    }

    private static string GenerateRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
