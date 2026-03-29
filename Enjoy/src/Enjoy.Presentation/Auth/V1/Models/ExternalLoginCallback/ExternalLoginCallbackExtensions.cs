using Enjoy.Application.Auth.Commands.ExternalLoginCallback;

namespace Enjoy.Presentation.Auth.V1.Models.ExternalLoginCallback;

public static class ExternalLoginCallbackExtensions
{
    public static ExternalLoginCallbackCommand ToCommand(this ExternalLoginCallbackRequest request) =>
        new(request.Code, request.State ?? string.Empty, request.Provider);

    public static ExternalLoginCallbackResponse ToResponse(this ExternalLoginCallbackCommandResponse response) =>
        new(response.UserId, response.AccessToken, response.RefreshToken);
}
