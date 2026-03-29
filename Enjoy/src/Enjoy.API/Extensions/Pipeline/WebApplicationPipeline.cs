using Enjoy.API.Configurations;
using Enjoy.Presentation.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Enjoy.API.Extensions.Pipeline;

public static class WebApplicationPipelineExtensions
{
    public static WebApplication UseHttpPipeline(this WebApplication app, IWebHostEnvironment env)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        app.UseSerilogRequestLogging();
        app.UseExceptionHandler();

        if (!env.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(CorsOptions.PolicyName);
        app.UseAuthentication();
        app.UseMiddleware<UserContextEnrichmentMiddleware>();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();

        return app;
    }
}
