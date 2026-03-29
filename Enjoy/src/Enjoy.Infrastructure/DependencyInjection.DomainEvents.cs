using Enjoy.Infrastructure.Idempotence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDomainEventPipeline(this IServiceCollection services)
    {
        services.TryDecorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));

        return services;
    }
}
