using Enjoy.Application.Abstractions.Math;
using Enjoy.Application.Abstractions.Notifications;
using Enjoy.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Enjoy.Application;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IMathService, MathService>();

        services.AddSingleton<INotifier, EmailNotifier>();
        services.AddSingleton<INotifier, SmsNotifier>();
        services.AddSingleton<IAlertService, AlertService>();

        return services;
    }
}
