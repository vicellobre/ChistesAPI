namespace Enjoy.Application.Abstractions.ExternalAuth;

public interface IExternalAuthProvider
{
    Uri BuildAuthorizationUrl(string provider, string state);

    Task<ExternalUserInfo?> ExchangeCodeForUserInfoAsync(
        string provider,
        string code,
        CancellationToken cancellationToken = default);
}
