using Dapper;
using Enjoy.Application.Abstractions.Clock;
using Enjoy.Application.Abstractions.Data;
using Enjoy.Application.Abstractions.Email;
using Enjoy.Infrastructure.Clock;
using Enjoy.Infrastructure.Data;
using Enjoy.Infrastructure.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDataAndEmailServices(this IServiceCollection services)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IEmailService, EmailService>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }
}
