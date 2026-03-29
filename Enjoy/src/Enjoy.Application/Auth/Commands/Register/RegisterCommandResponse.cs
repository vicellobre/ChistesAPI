namespace Enjoy.Application.Auth.Commands.Register;

public sealed record RegisterCommandResponse(
    string UserId,
    string AccessToken,
    string RefreshToken);
