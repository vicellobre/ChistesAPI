using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Jokes.V1.Models.GetRandomJoke;

/// <summary>Optional filter by random joke origin.</summary>
/// <param name="Origin">Origin: e.g. <c>Chuck</c> or <c>Dad</c>. Omit for any.</param>
public sealed record GetRandomJokeRequest(
    [property: FromQuery(Name = "origin")] string? Origin);
