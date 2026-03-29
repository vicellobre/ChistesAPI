using System.Security.Claims;
using Enjoy.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Enjoy.Infrastructure.Auth;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? GetUserId() =>
        httpContextAccessor.HttpContext?.User.GetIdentityId();

    public string? GetUserRole() =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}
