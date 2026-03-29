using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Jokes.Queries.FilterJokes;

public sealed record FilterJokesQuery(
    int? MinWords,
    string? Contains,
    string? AuthorId,
    string? TopicId) : IQuery<FilterJokesQueryResponse>;
