using Enjoy.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.API.Extensions.ServiceCollection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(AssemblyReference.Assembly);

        return services;
    }
}
