using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Application;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddApplicationMessaging();
        services.AddApplicationServices();
        return services;
    }
}
