namespace Enjoy.Presentation.Users.V1.Models.GetAdmin;

public static class GetAdminExtensions
{
    public static GetAdminResponse ToResponse(this GetAdminRequest _) =>
        new("Admin endpoint enabled.");
}
