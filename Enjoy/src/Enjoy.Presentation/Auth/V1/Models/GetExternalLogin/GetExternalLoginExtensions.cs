using Enjoy.Application.Auth.Queries.ExternalLogin;

namespace Enjoy.Presentation.Auth.V1.Models.GetExternalLogin;

public static class GetExternalLoginExtensions
{
    public static GetExternalLoginQuery ToQuery(this GetExternalLoginRequest request) =>
        new(request.Provider);

    public static GetExternalLoginResponse ToResponse(this GetExternalLoginQueryResponse response) =>
        new(response.RedirectUrl.AbsoluteUri);
}
