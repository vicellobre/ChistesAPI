using Enjoy.Application.Maths.Queries.GetNextNumber;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Presentation.Maths.V1.Models.GetNextNumber;

public static class GetNextNumberExtensions
{
    public static Result<GetNextNumberQuery> ToQuery(this GetNextNumberRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Number is not int number)
        {
            return Result.Failure<GetNextNumberQuery>(
                Error.Validation("Math.NumberMissing", "Query param 'number' is required."));
        }

        return Result.Success(new GetNextNumberQuery(number));
    }

    public static GetNextNumberResponse ToResponse(this GetNextNumberQueryResponse response) =>
        new(response.NextNumber);
}
