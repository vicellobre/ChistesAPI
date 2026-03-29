namespace Enjoy.Presentation.Auth.V1.Models.Login;

/// <summary>Sign-in credentials (JSON body for POST <c>/api/auth/login</c>).</summary>
/// <param name="Email">User email.</param>
/// <param name="Password">Plain-text password (use HTTPS in production).</param>
/// <remarks>
/// <code language="json">
/// {
///   "email": "user@localhost.dev",
///   "password": "User12345!"
/// }
/// </code>
/// </remarks>
public sealed record LoginRequest(string Email, string Password);
