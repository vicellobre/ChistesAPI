using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Auth.Commands.Register;

internal sealed class RegisterCommandHandler(
    IUserRegistrationService userRegistrationService) : ICommandHandler<RegisterCommand, RegisterCommandResponse>
{
    public async Task<Result<RegisterCommandResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await userRegistrationService.RegisterAsync(
            request.Name,
            request.Email,
            request.Password,
            cancellationToken);

        if (result.IsFailure)
            return Result.Failure<RegisterCommandResponse>(result.Errors);

        var auth = result.Value;
        return Result.Success(new RegisterCommandResponse(auth.UserId, auth.AccessToken, auth.RefreshToken));
    }
}
