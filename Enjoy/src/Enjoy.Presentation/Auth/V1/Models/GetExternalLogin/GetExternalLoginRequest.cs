using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Auth.V1.Models.GetExternalLogin;

/// <summary>Provider taken from route <c>external/{provider}-login</c> (reference; the controller binds <c>string provider</c>).</summary>
/// <param name="Provider">Segment before <c>-login</c>. Example: <c>github</c>.</param>
public sealed record GetExternalLoginRequest(
    [property: FromRoute(Name = "provider")] string Provider);
