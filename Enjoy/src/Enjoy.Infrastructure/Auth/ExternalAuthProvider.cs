using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Enjoy.Application.Abstractions.ExternalAuth;
using Enjoy.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;

namespace Enjoy.Infrastructure.Auth;

internal sealed class ExternalAuthProvider(
    HttpClient httpClient,
    IOptions<ExternalAuthOptions> options) : IExternalAuthProvider
{
    private readonly ExternalAuthOptions _options = options.Value;

    public Uri BuildAuthorizationUrl(string provider, string state)
    {
        if (string.Equals(provider, GoogleProviderOptions.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            var g = _options.Google;
            if (string.IsNullOrEmpty(g.ClientId) || string.IsNullOrEmpty(g.RedirectUri))
                throw new InvalidOperationException("ExternalAuth:Google:ClientId and RedirectUri must be configured.");

            var query = new Dictionary<string, string>
            {
                ["client_id"] = g.ClientId,
                ["redirect_uri"] = g.RedirectUri,
                ["response_type"] = "code",
                ["scope"] = "openid email profile",
                ["state"] = state
            };
            string queryString = string.Join("&", query.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var builder = new UriBuilder(GoogleOAuthConstants.AuthorizationEndpoint) { Query = queryString };
            return builder.Uri;
        }

        if (string.Equals(provider, GitHubProviderOptions.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            var gh = _options.GitHub;
            if (string.IsNullOrEmpty(gh.ClientId) || string.IsNullOrEmpty(gh.RedirectUri))
                throw new InvalidOperationException("ExternalAuth:GitHub:ClientId and RedirectUri must be configured.");

            var query = new Dictionary<string, string>
            {
                ["client_id"] = gh.ClientId,
                ["redirect_uri"] = gh.RedirectUri,
                ["scope"] = "read:user user:email",
                ["state"] = state
            };
            string queryString = string.Join("&", query.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var builder = new UriBuilder(GitHubOAuthConstants.AuthorizationEndpoint) { Query = queryString };
            return builder.Uri;
        }

        throw new NotSupportedException($"External provider '{provider}' is not supported.");
    }

    public async Task<ExternalUserInfo?> ExchangeCodeForUserInfoAsync(
        string provider,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.Equals(provider, GoogleProviderOptions.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            var g = _options.Google;
            if (string.IsNullOrEmpty(g.ClientId) || string.IsNullOrEmpty(g.ClientSecret) || string.IsNullOrEmpty(g.RedirectUri))
                return null;

            // 1. Exchange code for access token
            using var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = g.ClientId,
                ["client_secret"] = g.ClientSecret,
                ["redirect_uri"] = g.RedirectUri,
                ["grant_type"] = "authorization_code"
            });

            using var tokenResponse = await httpClient.PostAsync(
                GoogleOAuthConstants.TokenEndpoint,
                tokenRequest,
                cancellationToken);

            if (!tokenResponse.IsSuccessStatusCode)
                return null;

            var tokenPayload = await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken);
            string? accessToken = tokenPayload?.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
                return null;

            // 2. Get user info
            using var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, GoogleOAuthConstants.UserInformationEndpoint);
            userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var userInfoResponse = await httpClient.SendAsync(userInfoRequest, cancellationToken);
            if (!userInfoResponse.IsSuccessStatusCode)
                return null;

            var userInfoPayload = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfoResponse>(cancellationToken);
            if (string.IsNullOrEmpty(userInfoPayload?.Email))
                return null;

            return new ExternalUserInfo(
                userInfoPayload.Name ?? userInfoPayload.Email,
                userInfoPayload.Email);
        }

        if (string.Equals(provider, GitHubProviderOptions.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            var gh = _options.GitHub;
            if (string.IsNullOrEmpty(gh.ClientId) || string.IsNullOrEmpty(gh.ClientSecret) || string.IsNullOrEmpty(gh.RedirectUri))
                return null;

            // 1. Exchange code for access token
            using var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = gh.ClientId,
                ["client_secret"] = gh.ClientSecret,
                ["code"] = code,
                ["redirect_uri"] = gh.RedirectUri
            });

            using var tokenResponse = await httpClient.PostAsync(
                GitHubOAuthConstants.TokenEndpoint,
                tokenRequest,
                cancellationToken);

            if (!tokenResponse.IsSuccessStatusCode)
                return null;

            var tokenPayload = await tokenResponse.Content.ReadFromJsonAsync<GitHubTokenResponse>(cancellationToken);
            string? accessToken = tokenPayload?.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
                return null;

            // 2. Get user info (name + login)
            using var userRequest = new HttpRequestMessage(HttpMethod.Get, GitHubOAuthConstants.UserEndpoint);
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            userRequest.Headers.UserAgent.ParseAdd("EnjoyApp");

            using var userResponse = await httpClient.SendAsync(userRequest, cancellationToken);
            if (!userResponse.IsSuccessStatusCode)
                return null;

            var userPayload = await userResponse.Content.ReadFromJsonAsync<GitHubUserResponse>(cancellationToken);
            if (userPayload is null)
                return null;

            // 3. Get email(s) to pick the primary/verified one
            using var emailsRequest = new HttpRequestMessage(HttpMethod.Get, GitHubOAuthConstants.UserEmailsEndpoint);
            emailsRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            emailsRequest.Headers.UserAgent.ParseAdd("EnjoyApp");

            using var emailsResponse = await httpClient.SendAsync(emailsRequest, cancellationToken);
            if (!emailsResponse.IsSuccessStatusCode)
                return null;

            var emailsPayload = await emailsResponse.Content.ReadFromJsonAsync<List<GitHubEmailResponse>>(cancellationToken);
            string? email = emailsPayload?
                .FirstOrDefault(e => e.Primary == true && e.Verified == true)?.Email
                ?? emailsPayload?
                    .FirstOrDefault(e => e.Verified == true)?.Email
                ?? emailsPayload?.FirstOrDefault()?.Email;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            string name = !string.IsNullOrWhiteSpace(userPayload.Name)
                ? userPayload.Name
                : userPayload.Login;

            if (string.IsNullOrWhiteSpace(name))
                return null;

            return new ExternalUserInfo(name, email);
        }

        return null;
    }

    private sealed class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    private sealed class GoogleUserInfoResponse
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    private sealed class GitHubTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    private sealed class GitHubUserResponse
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    private sealed class GitHubEmailResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("primary")]
        public bool? Primary { get; set; }

        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }
    }
}

/// <summary>
/// Google OAuth 2.0 endpoints (stable). Avoids pulling in Microsoft.AspNetCore.Authentication.Google.
/// </summary>
internal static class GoogleOAuthConstants
{
    public const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    public const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    public const string UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
}

/// <summary>
/// GitHub OAuth constants and API endpoints.
/// </summary>
internal static class GitHubOAuthConstants
{
    public const string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    public const string TokenEndpoint = "https://github.com/login/oauth/access_token";
    public const string UserEndpoint = "https://api.github.com/user";
    public const string UserEmailsEndpoint = "https://api.github.com/user/emails";
}
