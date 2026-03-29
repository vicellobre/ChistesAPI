namespace Enjoy.Application.Auth.Commands.ExternalLoginCallback;

public sealed record ExternalLoginCallbackCommandResponse(
    string AccessToken,
    string RefreshToken,
    string UserId);
