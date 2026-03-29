using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Abstractions.Authentication;

public interface IAuthResultIssuer
{
    Task<Result<AuthResult>> IssueAsync(
        string userId,
        string email,
        IEnumerable<string> roles,
        Guid identityUserId,
        CancellationToken cancellationToken = default);
}
