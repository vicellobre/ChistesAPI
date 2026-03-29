using Enjoy.Application.Jokes.Queries.GetCombinedJoke;

namespace Enjoy.Presentation.Jokes.V1.Models.GetCombinedJoke;

public static class GetCombinedJokeExtensions
{
    public static GetCombinedJokeQuery ToQuery(this GetCombinedJokeRequest _) =>
        new();

    public static GetCombinedJokeResponse ToResponse(this GetCombinedJokeQueryResponse response) =>
        new(
            response.Text,
            response.SourceFragments
                .Select(f => new CombinedJokeSourceItem(f.Source, f.Fragment))
                .ToList());
}
