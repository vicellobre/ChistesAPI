namespace Enjoy.Presentation.Jokes.V1.Models.CreateJoke;

/// <summary>Persisted joke identifier.</summary>
/// <param name="JokeId">Domain id (e.g. <c>jke_...</c>).</param>
public sealed record CreateJokeResponse(string JokeId);
