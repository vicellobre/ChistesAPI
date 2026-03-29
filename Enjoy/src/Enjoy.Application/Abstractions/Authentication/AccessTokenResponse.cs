namespace Enjoy.Application.Abstractions.Authentication;

public sealed record AccessTokenResponse(string AccessToken, string RefreshToken);
