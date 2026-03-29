using Enjoy.Persistence.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Persistence;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPersistenceInterceptors(this IServiceCollection services)
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

        return services;
    }
}
