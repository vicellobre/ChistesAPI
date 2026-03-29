using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Results;

namespace Enjoy.Application.Admin.Queries.GetAdminResource;

internal sealed class GetAdminResourceQueryHandler
    : IQueryHandler<GetAdminResourceQuery, GetAdminResourceQueryResponse>
{
    public Task<Result<GetAdminResourceQueryResponse>> Handle(
        GetAdminResourceQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetAdminResourceQueryResponse(
            "Welcome, Admin! This is a protected admin resource.",
            DateTime.UtcNow);

        return Task.FromResult(Result.Success(response));
    }
}
