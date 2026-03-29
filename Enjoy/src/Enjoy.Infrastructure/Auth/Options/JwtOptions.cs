namespace Enjoy.Infrastructure.Auth.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int TokenExpirationInMinutes { get; init; } = 60;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}
