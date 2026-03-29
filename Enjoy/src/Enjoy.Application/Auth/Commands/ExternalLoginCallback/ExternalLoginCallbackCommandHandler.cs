using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Abstractions.ExternalAuth;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Abstractions;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.Repositories;
using Enjoy.Domain.Users.ValueObjects;

namespace Enjoy.Application.Auth.Commands.ExternalLoginCallback;

internal sealed class ExternalLoginCallbackCommandHandler(
    IExternalAuthProvider externalAuthProvider,
    IUserRepository userRepository,
    IUserRegistrationService userRegistrationService,
    IAuthResultIssuer authResultIssuer,
    IUnitOfWork unitOfWork) : ICommandHandler<ExternalLoginCallbackCommand, ExternalLoginCallbackCommandResponse>
{
    public async Task<Result<ExternalLoginCallbackCommandResponse>> Handle(
        ExternalLoginCallbackCommand request,
        CancellationToken cancellationToken)
    {
        var userInfo = await externalAuthProvider.ExchangeCodeForUserInfoAsync(
            request.Provider, request.Code, cancellationToken);

        if (userInfo is null)
            return Result.Failure<ExternalLoginCallbackCommandResponse>(
                Error.Unauthorized("Auth.ExternalLoginFailed", "Failed to retrieve user information from the external provider."));

        var existingUser = await userRepository.GetByEmailAsync(userInfo.Email);

        if (existingUser is null)
        {
            var registerResult = await userRegistrationService.RegisterExternalAsync(
                userInfo.Name, userInfo.Email, cancellationToken);

            if (registerResult.IsFailure)
                return Result.Failure<ExternalLoginCallbackCommandResponse>(registerResult.Errors);

            var auth = registerResult.Value;
            return Result.Success(new ExternalLoginCallbackCommandResponse(
                auth.AccessToken, auth.RefreshToken, auth.UserId));
        }

        existingUser.ChangeName(userInfo.Name);
        await userRepository.UpdateAsync(existingUser);
        await unitOfWork.CommitAsync(cancellationToken);

        var authResult = await authResultIssuer.IssueAsync(
            existingUser.Id,
            existingUser.Email.Value,
            [existingUser.Role.Value],
            existingUser.IdentityId,
            cancellationToken);
        if (authResult.IsFailure)
            return Result.Failure<ExternalLoginCallbackCommandResponse>(authResult.Errors);

        var a = authResult.Value;
        return Result.Success(new ExternalLoginCallbackCommandResponse(
            a.AccessToken, a.RefreshToken, a.UserId));
    }
}
