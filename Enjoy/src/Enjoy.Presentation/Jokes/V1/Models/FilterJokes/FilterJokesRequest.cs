using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Jokes.V1.Models.FilterJokes;

/// <summary>List filters (all optional).</summary>
/// <param name="MinWords">Minimum word count in text.</param>
/// <param name="Contains">Substring that must appear in text.</param>
/// <param name="AuthorId">Filter by author (<c>usr_...</c>).</param>
/// <param name="TopicId">Filter by associated topic.</param>
public sealed record FilterJokesRequest(
    [property: FromQuery] int? MinWords,
    [property: FromQuery] string? Contains,
    [property: FromQuery] string? AuthorId,
    [property: FromQuery] string? TopicId);
