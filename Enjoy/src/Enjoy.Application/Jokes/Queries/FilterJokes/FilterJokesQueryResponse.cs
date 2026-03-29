namespace Enjoy.Application.Jokes.Queries.FilterJokes;

public sealed record FilterJokesQueryResponse(IReadOnlyList<JokeListItem> Jokes);
