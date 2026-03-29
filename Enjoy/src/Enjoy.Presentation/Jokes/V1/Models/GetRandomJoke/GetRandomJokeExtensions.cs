using Enjoy.Application.Jokes.Queries.GetRandomJoke;

namespace Enjoy.Presentation.Jokes.V1.Models.GetRandomJoke;

public static class GetRandomJokeExtensions
{
    public static GetRandomJokeQuery ToQuery(this GetRandomJokeRequest request) =>
        new(request.Origin);

    public static GetRandomJokeResponse ToResponse(this GetRandomJokeQueryResponse response) =>
        new(response.Text, response.Origin);
}
