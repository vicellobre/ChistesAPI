namespace Enjoy.Application.Jokes.Queries.GetCombinedJoke;

public sealed record GetCombinedJokeQueryResponse(string Text, IReadOnlyList<CombinedJokeSourceFragment> SourceFragments);
