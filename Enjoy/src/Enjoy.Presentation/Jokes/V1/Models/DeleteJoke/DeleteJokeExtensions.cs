using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Jokes.Commands.DeleteJoke;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Presentation.Jokes.V1.Models.DeleteJoke;

public static class DeleteJokeExtensions
{
    public static Result<DeleteJokeCommand> ToCommand(this DeleteJokeRequest request, IUserContext userContext)
    {
        var userId = userContext.GetUserId();
        var role = userContext.GetUserRole();
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
        {
            return Result.Failure<DeleteJokeCommand>(
                Error.Unauthorized("Auth.Unauthenticated", "User is not authenticated."));
        }

        return new DeleteJokeCommand(request.Id, userId, role);
    }
}
