using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Abstractions.Authentication;

public interface IUserRegistrationService
{
    Task<Result<AuthResult>> RegisterAsync(
        string name,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResult>> RegisterAsync(
        string name,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResult>> RegisterExternalAsync(
        string name,
        string email,
        CancellationToken cancellationToken = default);
}
