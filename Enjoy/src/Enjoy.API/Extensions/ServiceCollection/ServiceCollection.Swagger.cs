using Enjoy.API.Configurations.Swagger;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.API.Extensions.ServiceCollection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services.ConfigureOptions<ConfigureSwaggerUIOptions>();

        return services;
    }
}
