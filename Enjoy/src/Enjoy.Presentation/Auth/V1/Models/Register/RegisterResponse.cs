namespace Enjoy.Presentation.Auth.V1.Models.Register;

/// <summary>Registration response: domain id and tokens.</summary>
/// <param name="UserId">Id with <c>usr_</c> prefix.</param>
/// <param name="AccessToken">Access JWT.</param>
/// <param name="RefreshToken">Refresh token.</param>
/// <remarks>
/// <code language="json">
/// {
///   "userId": "usr_019d...",
///   "accessToken": "eyJ...",
///   "refreshToken": "..."
/// }
/// </code>
/// </remarks>
public sealed record RegisterResponse(string UserId, string AccessToken, string RefreshToken);
