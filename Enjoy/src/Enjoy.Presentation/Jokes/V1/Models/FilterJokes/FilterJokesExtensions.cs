using Enjoy.Application.Jokes.Queries.FilterJokes;
using Enjoy.Presentation.Jokes.V1.Models;
using Enjoy.Presentation.Jokes.V1.Models.Common;

namespace Enjoy.Presentation.Jokes.V1.Models.FilterJokes;

public static class FilterJokesExtensions
{
    public static FilterJokesQuery ToQuery(this FilterJokesRequest request) =>
        new(request.MinWords, request.Contains, request.AuthorId, request.TopicId);

    public static FilterJokesResponse ToResponse(this FilterJokesQueryResponse response) =>
        new(response.Jokes.Select(j => new JokeResponse(
                j.Id,
                j.Text,
                j.AuthorId,
                j.Origin,
                j.TopicIds))
            .ToList());
}
