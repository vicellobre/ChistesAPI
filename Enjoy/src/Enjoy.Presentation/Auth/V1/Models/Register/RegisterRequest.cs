namespace Enjoy.Presentation.Auth.V1.Models.Register;

/// <summary>User sign-up (POST <c>/api/auth/register</c>).</summary>
/// <param name="Name">Display name.</param>
/// <param name="Email">Unique email.</param>
/// <param name="Password">Password (Identity policy).</param>
/// <param name="ConfirmPassword">Must match <paramref name="Password"/>.</param>
/// <remarks>
/// <code language="json">
/// {
///   "name": "Jane Demo",
///   "email": "newuser@localhost.dev",
///   "password": "User12345!",
///   "confirmPassword": "User12345!"
/// }
/// </code>
/// </remarks>
public sealed record RegisterRequest(string Name, string Email, string Password, string ConfirmPassword);
