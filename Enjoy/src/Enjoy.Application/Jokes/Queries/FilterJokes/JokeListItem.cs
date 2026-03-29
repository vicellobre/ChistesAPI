namespace Enjoy.Application.Jokes.Queries.FilterJokes;

public sealed record JokeListItem(
    string Id,
    string Text,
    string AuthorId,
    string Origin,
    IReadOnlyCollection<string> TopicIds);
