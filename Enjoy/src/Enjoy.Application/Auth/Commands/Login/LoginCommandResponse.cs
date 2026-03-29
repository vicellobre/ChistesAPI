namespace Enjoy.Application.Auth.Commands.Login;

public sealed record LoginCommandResponse(string AccessToken, string RefreshToken);
