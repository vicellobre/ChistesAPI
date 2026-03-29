namespace Enjoy.Presentation.Jokes.V1.Models.Common;

/// <summary>HTTP representation of a joke in lists / collections.</summary>
/// <param name="Id">Identifier.</param>
/// <param name="Text">Text.</param>
/// <param name="AuthorId">Author (<c>usr_...</c>).</param>
/// <param name="Origin">Content origin.</param>
/// <param name="TopicIds">Linked topics.</param>
public sealed record JokeResponse(
    string Id,
    string Text,
    string AuthorId,
    string Origin,
    IReadOnlyCollection<string> TopicIds);
