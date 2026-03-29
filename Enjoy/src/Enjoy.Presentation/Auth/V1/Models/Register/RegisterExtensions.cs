using Enjoy.Application.Auth.Commands.Register;

namespace Enjoy.Presentation.Auth.V1.Models.Register;

public static class RegisterExtensions
{
    public static RegisterCommand ToCommand(this RegisterRequest request) =>
        new(request.Name, request.Email, request.Password, request.ConfirmPassword);

    public static RegisterResponse ToResponse(this RegisterCommandResponse response) =>
        new(response.UserId, response.AccessToken, response.RefreshToken);
}
