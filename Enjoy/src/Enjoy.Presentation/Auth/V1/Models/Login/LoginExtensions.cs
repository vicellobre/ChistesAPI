using Enjoy.Application.Auth.Commands.Login;

namespace Enjoy.Presentation.Auth.V1.Models.Login;

public static class LoginExtensions
{
    public static LoginCommand ToCommand(this LoginRequest request) =>
        new(request.Email, request.Password);

    public static LoginResponse ToResponse(this LoginCommandResponse response) =>
        new(response.AccessToken, response.RefreshToken);
}
