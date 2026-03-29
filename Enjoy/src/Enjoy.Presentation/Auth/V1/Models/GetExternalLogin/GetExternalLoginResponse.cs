namespace Enjoy.Presentation.Auth.V1.Models.GetExternalLogin;

/// <summary>Absolute URL to the OAuth provider consent screen.</summary>
/// <param name="RedirectUrl">Includes <c>client_id</c>, <c>redirect_uri</c>, <c>scope</c>, <c>state</c> in the query string.</param>
/// <remarks>
/// <code language="json">
/// {
///   "redirectUrl": "https://github.com/login/oauth/authorize?client_id=...&amp;redirect_uri=...&amp;scope=...&amp;state=..."
/// }
/// </code>
/// </remarks>
public sealed record GetExternalLoginResponse(string RedirectUrl);
