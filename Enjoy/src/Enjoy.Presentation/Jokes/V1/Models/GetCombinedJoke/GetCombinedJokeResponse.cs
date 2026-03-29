namespace Enjoy.Presentation.Jokes.V1.Models.GetCombinedJoke;

/// <summary>Combined joke with source traces.</summary>
/// <param name="Text">Final displayed text.</param>
/// <param name="SourceFragments">Fragments per source (origin + text).</param>
public sealed record GetCombinedJokeResponse(string Text, IReadOnlyList<CombinedJokeSourceItem> SourceFragments);
