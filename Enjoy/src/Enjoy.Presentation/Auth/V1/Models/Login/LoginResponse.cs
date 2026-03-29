namespace Enjoy.Presentation.Auth.V1.Models.Login;

/// <summary>Tokens issued after a successful sign-in.</summary>
/// <param name="AccessToken">Access JWT (send as <c>Bearer</c>).</param>
/// <param name="RefreshToken">Refresh token.</param>
/// <remarks>
/// <code language="json">
/// {
///   "accessToken": "eyJhbGciOi...",
///   "refreshToken": "CfDJ8..."
/// }
/// </code>
/// </remarks>
public sealed record LoginResponse(string AccessToken, string RefreshToken);
