using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Jokes.Queries.GetRandomJoke;

public sealed record GetRandomJokeQuery(string? Origin) : IQuery<GetRandomJokeQueryResponse>;
