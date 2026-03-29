using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Application.Users.Queries.GetCurrentUser;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Presentation.Users.V1.Models.GetCurrentUser;

public static class GetCurrentUserExtensions
{
    public static Result<GetCurrentUserQuery> ToQuery(this GetCurrentUserRequest _, IUserContext userContext)
    {
        var userId = userContext.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure<GetCurrentUserQuery>(
                Error.Unauthorized("Auth.Unauthenticated", "User is not authenticated."));
        }

        return new GetCurrentUserQuery(userId);
    }

    public static GetCurrentUserResponse ToResponse(this GetCurrentUserQueryResponse response) =>
        new(response.Id, response.Name, response.Email, response.Role);
}
