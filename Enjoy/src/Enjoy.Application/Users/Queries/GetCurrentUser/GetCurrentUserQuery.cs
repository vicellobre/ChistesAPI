using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(string UserId) : IQuery<GetCurrentUserQueryResponse>;
