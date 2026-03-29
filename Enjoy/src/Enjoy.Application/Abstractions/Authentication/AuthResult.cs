namespace Enjoy.Application.Abstractions.Authentication;

public sealed record AuthResult(
    string UserId,
    string AccessToken,
    string RefreshToken);
