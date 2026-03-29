using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Jokes.Entities;
using Enjoy.Domain.Jokes.Repositories;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Jokes.Queries.FilterJokes;

internal sealed class FilterJokesQueryHandler(
    IJokeRepository jokeRepository) : IQueryHandler<FilterJokesQuery, FilterJokesQueryResponse>
{
    public async Task<Result<FilterJokesQueryResponse>> Handle(
        FilterJokesQuery request,
        CancellationToken cancellationToken)
    {
        var allJokes = await jokeRepository.ListAsync();
        IEnumerable<Joke> filtered = allJokes;

        if (request.MinWords.HasValue)
            filtered = filtered.Where(j => CountWords(j.Text.Value) >= request.MinWords.Value);

        if (!string.IsNullOrWhiteSpace(request.Contains))
            filtered = filtered.Where(j => j.Text.Value.Contains(request.Contains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.AuthorId))
            filtered = filtered.Where(j => j.AuthorId == request.AuthorId);

        if (!string.IsNullOrWhiteSpace(request.TopicId))
            filtered = filtered.Where(j => j.Topics.Any(t => t.Id == request.TopicId));

        var items = filtered
            .Select(j => new JokeListItem(j.Id, j.Text.Value, j.AuthorId, j.Origin.Value, j.TopicIds))
            .ToList();

        return Result.Success(new FilterJokesQueryResponse(items));
    }

    private static int CountWords(string text) =>
        text.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
}
