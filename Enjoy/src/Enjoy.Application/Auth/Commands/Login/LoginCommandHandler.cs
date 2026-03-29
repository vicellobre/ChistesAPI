using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.Repositories;

namespace Enjoy.Application.Auth.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    public async Task<Result<LoginCommandResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<LoginCommandResponse>(Error.User.InvalidCredentials);

        if (!passwordHasher.Verify(request.Password, user.PasswordHash.Value))
            return Result.Failure<LoginCommandResponse>(Error.User.InvalidCredentials);

        var tokens = tokenProvider.Create(user.Id, user.Email.Value, [user.Role.Value]);

        return Result.Success(new LoginCommandResponse(tokens.AccessToken, tokens.RefreshToken));
    }
}
