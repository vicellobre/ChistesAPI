using Enjoy.Application.Abstractions.ExternalAuth;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Auth.Queries.ExternalLogin;

internal sealed class GetExternalLoginQueryHandler(
    IExternalAuthProvider externalAuthProvider) : IQueryHandler<GetExternalLoginQuery, GetExternalLoginQueryResponse>
{
    public Task<Result<GetExternalLoginQueryResponse>> Handle(
        GetExternalLoginQuery request,
        CancellationToken cancellationToken)
    {
        string state = Guid.NewGuid().ToString("N");
        Uri redirectUrl = externalAuthProvider.BuildAuthorizationUrl(request.Provider, state);

        return Task.FromResult(
            Result.Success(new GetExternalLoginQueryResponse(redirectUrl)));
    }
}
