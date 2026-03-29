namespace Enjoy.Presentation.Jokes.V1.Models.GetCombinedJoke;

/// <summary>Fragment attributed to a source in the combined joke.</summary>
/// <param name="Source">Origin label (e.g. <c>Chuck</c>).</param>
/// <param name="Fragment">Snippet used in fusion.</param>
public sealed record CombinedJokeSourceItem(string Source, string Fragment);
