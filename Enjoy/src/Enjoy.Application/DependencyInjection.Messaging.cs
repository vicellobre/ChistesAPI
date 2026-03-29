using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Application;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApplicationMessaging(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
