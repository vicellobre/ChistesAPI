namespace Enjoy.Application.Jokes.Queries.GetPairedJokes;

public sealed record GetPairedJokesQueryResponse(IReadOnlyList<PairedJokeItem> Pairs);
