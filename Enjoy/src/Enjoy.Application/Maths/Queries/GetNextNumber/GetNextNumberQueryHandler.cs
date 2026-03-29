using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Maths.Queries.GetNextNumber;

internal sealed class GetNextNumberQueryHandler : IQueryHandler<GetNextNumberQuery, GetNextNumberQueryResponse>
{
    public Task<Result<GetNextNumberQueryResponse>> Handle(
        GetNextNumberQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetNextNumberQueryResponse(request.Number + 1);

        return Task.FromResult(Result.Success(response));
    }
}
