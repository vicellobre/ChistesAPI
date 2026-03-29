using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Application.Abstractions.Math;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Maths.Queries.GetLeastCommonMultiple;

internal sealed class GetLeastCommonMultipleQueryHandler(
    IMathService mathService) : IQueryHandler<GetLeastCommonMultipleQuery, GetLeastCommonMultipleQueryResponse>
{
    public Task<Result<GetLeastCommonMultipleQueryResponse>> Handle(
        GetLeastCommonMultipleQuery request,
        CancellationToken cancellationToken)
    {
        var numbers = request.Numbers.Select(n => (long)n);

        long result = mathService.LeastCommonMultiple(numbers);

        return Task.FromResult(
            Result.Success(new GetLeastCommonMultipleQueryResponse(result)));
    }
}
