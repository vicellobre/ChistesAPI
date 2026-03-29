using Enjoy.Domain.Shared.Errors;
using Microsoft.AspNetCore.Http;

namespace Enjoy.Presentation.Errors.Extensions;

public static class ErrorTypeExtensions
{
    public static int ToStatusCode(this ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            ErrorType.Exception => StatusCodes.Status500InternalServerError,
            ErrorType.None => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };
}
