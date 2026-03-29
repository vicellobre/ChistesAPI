using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Auth.V1.Models.ExternalLoginCallback;

/// <summary>OAuth redirect parameters (<c>GET /api/auth/external/callback</c>).</summary>
/// <param name="Code">Authorization code from the provider.</param>
/// <param name="State">Anti-CSRF value returned by the provider.</param>
/// <param name="Provider">Provider name. Example: <c>GitHub</c>.</param>
public sealed record ExternalLoginCallbackRequest(
    [property: FromQuery(Name = "code")] string Code,
    [property: FromQuery(Name = "state")] string? State,
    [property: FromQuery(Name = "provider")] string Provider);
