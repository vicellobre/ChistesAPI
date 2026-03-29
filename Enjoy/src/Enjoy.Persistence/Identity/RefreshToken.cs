using Microsoft.AspNetCore.Identity;

namespace Enjoy.Persistence.Identity;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }

    public IdentityUser User { get; set; } = null!;
}
