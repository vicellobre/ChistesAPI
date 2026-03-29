namespace Enjoy.Presentation.Jokes.V1.Models.CreateJoke;

/// <summary>POST <c>/api/chistes</c> body (<c>application/json</c>).</summary>
/// <param name="Text">Joke text.</param>
/// <remarks>
/// <code language="json">
/// {
///   "text": "Why do programmers confuse Halloween and Christmas? Because Oct 31 == Dec 25."
/// }
/// </code>
/// </remarks>
public sealed record CreateJokeRequest(string Text);
