using System.Diagnostics;
using Enjoy.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Enjoy.Presentation.Middlewares;

public sealed class UserContextEnrichmentMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext context, IUserContext userContext)
    {
        string? userId = userContext.GetUserId();
        if (userId is null)
        {
            return next(context);
        }

        Activity.Current?.SetTag("user.id", userId);

        using (LogContext.PushProperty("UserId", userId))
        {
            return next(context);
        }
    }
}
