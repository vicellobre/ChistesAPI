using Enjoy.Application.Jokes.Queries.GetPairedJokes;

namespace Enjoy.Presentation.Jokes.V1.Models.GetPairedJokes;

public static class GetPairedJokesExtensions
{
    public static GetPairedJokesQuery ToQuery(this GetPairedJokesRequest _) =>
        new();

    public static GetPairedJokesResponse ToResponse(this GetPairedJokesQueryResponse response) =>
        new(response.Pairs.Select(p => new PairedJokeResponse(p.Chuck, p.Dad, p.Combined)).ToList());
}
