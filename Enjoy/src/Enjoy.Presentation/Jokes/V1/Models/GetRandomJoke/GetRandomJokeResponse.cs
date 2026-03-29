namespace Enjoy.Presentation.Jokes.V1.Models.GetRandomJoke;

/// <summary>Resolved random joke.</summary>
/// <param name="Text">Joke text.</param>
/// <param name="Origin">Origin (<c>Chuck</c>, <c>Dad</c>, etc.).</param>
public sealed record GetRandomJokeResponse(string Text, string Origin);
