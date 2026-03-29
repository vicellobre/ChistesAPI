namespace Enjoy.Presentation.Users.V1.Models.GetCurrentUser;

/// <summary>Authenticated user profile.</summary>
/// <param name="Id">Domain id (<c>usr_...</c>).</param>
/// <param name="Name">Name.</param>
/// <param name="Email">Email.</param>
/// <param name="Role"><c>User</c> or <c>Admin</c>.</param>
public sealed record GetCurrentUserResponse(string Id, string Name, string Email, string Role);
