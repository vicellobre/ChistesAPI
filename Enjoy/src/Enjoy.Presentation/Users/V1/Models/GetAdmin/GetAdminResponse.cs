namespace Enjoy.Presentation.Users.V1.Models.GetAdmin;

/// <summary>Confirmation message for users with the Admin role.</summary>
/// <param name="Message">Informational text.</param>
public sealed record GetAdminResponse(string Message);
