using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Enjoy.API.Extensions.HostBuilder;

public static class HostBuilderSerilogExtensions
{
    public static ConfigureHostBuilder ConfigureSerilogFromConfiguration(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration));

        return host;
    }
}
