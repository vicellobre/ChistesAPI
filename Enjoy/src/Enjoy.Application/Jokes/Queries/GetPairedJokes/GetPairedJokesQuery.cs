using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Jokes.Queries.GetPairedJokes;

public sealed record GetPairedJokesQuery : IQuery<GetPairedJokesQueryResponse>;
