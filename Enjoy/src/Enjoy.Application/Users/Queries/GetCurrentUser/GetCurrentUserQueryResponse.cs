namespace Enjoy.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQueryResponse(
    string Id,
    string Name,
    string Email,
    string Role);
