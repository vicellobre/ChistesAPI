using Enjoy.Presentation.Jokes.V1.Models.Common;

namespace Enjoy.Presentation.Jokes.V1.Models.FilterJokes;

/// <summary>Joke filter result.</summary>
/// <param name="Jokes">Items matching the criteria.</param>
public sealed record FilterJokesResponse(IReadOnlyList<JokeResponse> Jokes);
