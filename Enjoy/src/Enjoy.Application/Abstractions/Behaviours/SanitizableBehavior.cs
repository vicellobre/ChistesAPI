using MediatR;

namespace Enjoy.Application.Abstractions.Behaviours;

public sealed class SanitizableBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    public SanitizableBehavior() { }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is ISanitizable filterableRequest)
        {
            filterableRequest.Sanitizable();
        }

        return await next(cancellationToken);
    }
}
