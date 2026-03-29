using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Jokes.V1.Models.DeleteJoke;

/// <summary>Delete by id in route.</summary>
/// <param name="Id">Joke id. Example: <c>jke_01...</c>.</param>
public sealed record DeleteJokeRequest(
    [property: FromRoute(Name = "id")] string Id);
