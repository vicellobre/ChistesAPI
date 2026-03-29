using Enjoy.API.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.API.Extensions.ServiceCollection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));

        CorsOptions cors =
            configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.PolicyName, policy =>
            {
                if (cors.AllowedOrigins is { Length: > 0 })
                {
                    policy.WithOrigins(cors.AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else if (cors.AllowAnyOriginWhenEmpty)
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Configure section '{CorsOptions.SectionName}' with non-empty '{nameof(CorsOptions.AllowedOrigins)}', " +
                        $"or set '{nameof(CorsOptions.AllowAnyOriginWhenEmpty)}' to true only for demo/open CORS.");
                }
            });
        });

        return services;
    }
}
