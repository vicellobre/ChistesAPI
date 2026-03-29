using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Presentation.Errors.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Problems;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create ProblemDetails from a successful result.");
        }

        var primary = result.FirstError;
        return new ProblemDetails
        {
            Title = primary.Code,
            Type = primary.Type.ToString(),
            Detail = primary.Message,
            Status = primary.Type.ToStatusCode(),
            Extensions = { { nameof(result.Errors), result.Errors } }
        };
    }

    public static ProblemDetails ToProblemDetails(this Error error) =>
        new()
        {
            Title = error.Code,
            Type = error.Type.ToString(),
            Detail = error.Message,
            Status = error.Type.ToStatusCode(),
            Extensions = { { nameof(error.StackTrace), error.StackTrace } }
        };
}
