using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.Repositories;

namespace Enjoy.Application.Users.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(
    IUserRepository userRepository) : IQueryHandler<GetCurrentUserQuery, GetCurrentUserQueryResponse>
{
    public async Task<Result<GetCurrentUserQueryResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<GetCurrentUserQueryResponse>(
                Error.NotFound("User.NotFound", $"User with ID '{request.UserId}' was not found."));

        return Result.Success(new GetCurrentUserQueryResponse(
            user.Id,
            user.Name.Value,
            user.Email.Value,
            user.Role.Value));
    }
}
