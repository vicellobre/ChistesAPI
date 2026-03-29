using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Jokes.V1.Models.UpdateJoke;

/// <summary>Update joke: id in route and new text in body.</summary>
/// <param name="Id">Joke id (route). Example: <c>jke_01...</c>.</param>
/// <param name="Payload">JSON body with <c>newText</c>.</param>
public sealed record UpdateJokeRequest(
    [property: FromRoute(Name = "id")] string Id,
    [property: FromBody] UpdateJokePayloadRequest Payload);

/// <summary>PUT <c>/api/chistes/{id}</c> body.</summary>
/// <param name="NewText">Full new joke text.</param>
public sealed record UpdateJokePayloadRequest(string NewText);
