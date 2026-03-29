namespace Enjoy.Presentation.Auth.V1.Models.ExternalLoginCallback;

/// <summary>Tokens after completing OAuth at the provider.</summary>
/// <param name="UserId">Domain id.</param>
/// <param name="AccessToken">Enjoy access JWT.</param>
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
public sealed record ExternalLoginCallbackResponse(string UserId, string AccessToken, string RefreshToken);
