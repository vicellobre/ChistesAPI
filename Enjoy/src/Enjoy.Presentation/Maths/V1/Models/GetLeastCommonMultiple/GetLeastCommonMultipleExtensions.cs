using Enjoy.Application.Maths.Queries.GetLeastCommonMultiple;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Presentation.Maths.V1.Models.GetLeastCommonMultiple;

public static class GetLeastCommonMultipleExtensions
{
    public static Result<GetLeastCommonMultipleQuery> ToQuery(this GetLeastCommonMultipleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Numbers))
        {
            return Result.Failure<GetLeastCommonMultipleQuery>(
                Error.Validation("Math.InvalidNumbers", "Query param 'numbers' is required."));
        }

        var parts = request.Numbers
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var numbers = new List<int>(parts.Length);

        foreach (var part in parts)
        {
            if (!int.TryParse(part, out var number))
            {
                return Result.Failure<GetLeastCommonMultipleQuery>(
                    Error.Validation("Math.InvalidNumbers", $"Value '{part}' is not a valid integer."));
            }

            numbers.Add(number);
        }

        return new GetLeastCommonMultipleQuery(numbers);
    }

    public static GetLeastCommonMultipleResponse ToResponse(this GetLeastCommonMultipleQueryResponse response) =>
        new(response.LeastCommonMultiple);
}
