using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Auth.Queries.ExternalLogin;

public sealed record GetExternalLoginQuery(string Provider) : IQuery<GetExternalLoginQueryResponse>;
