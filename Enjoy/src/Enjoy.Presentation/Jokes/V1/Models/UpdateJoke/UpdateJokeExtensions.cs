using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Jokes.Commands.UpdateJoke;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Presentation.Jokes.V1.Models.UpdateJoke;

public static class UpdateJokeExtensions
{
    public static Result<UpdateJokeCommand> ToCommand(this UpdateJokeRequest request, IUserContext userContext)
    {
        var userId = userContext.GetUserId();
        var role = userContext.GetUserRole();
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
        {
            return Result.Failure<UpdateJokeCommand>(
                Error.Unauthorized("Auth.Unauthenticated", "User is not authenticated."));
        }

        return new UpdateJokeCommand(request.Id, request.Payload.NewText, userId, role);
    }
}
