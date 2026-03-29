using System.Security.Claims;

namespace Enjoy.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string? GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
