using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.Shared.Errors;

public readonly partial record struct Error
{
    public string Code { get; init; }

    public string Message { get; init; }

    public ErrorType Type { get; init; }

    public IReadOnlyCollection<Error>? StackTrace { get; init; }

    public Error()
    {
        throw new InvalidOperationException("Use the static Create method to instantiate Error");
    }

    private Error(string? code, string? message, ErrorType type, IReadOnlyCollection<Error>? stack = null)
    {
        if (code is null)
        {
            throw new ArgumentNullException(nameof(code), "Error code cannot be null.");
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message), "Error message cannot be null.");
        }

        Code = !string.IsNullOrWhiteSpace(code) ? code : string.Empty;
        Message = !string.IsNullOrWhiteSpace(message) ? message : string.Empty;
        Type = type;
        StackTrace = stack ?? [];
    }

    private Error(IReadOnlyCollection<Error>? stack)
    {
        if (stack is null || stack.IsEmptyReadOnly())
        {
            throw new ArgumentNullException(nameof(stack), "Error stack cannot be null or empty.");
        }

        Code = stack.First().Code;
        Message = stack.First().Message;
        Type = stack.First().Type;
        StackTrace = stack;
    }

    public static Error Create(string? code, string? message, ErrorType type, IReadOnlyCollection<Error>? stack = null) => new(code, message, type, stack);

    public static Error WithStack(Error error, IReadOnlyCollection<Error>? stack) => new(error.Code, error.Message, error.Type, stack);

    public static Error WithStack(IReadOnlyCollection<Error>? errors) => new(errors);

    public static Error Failure(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Failure, stack);

    public static Error Unexpected(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Unexpected, stack);

    public static Error Validation(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Validation, stack);

    public static Error Conflict(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Conflict, stack);

    public static Error NotFound(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.NotFound, stack);

    public static Error Unauthorized(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Unauthorized, stack);

    public static Error Forbidden(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Forbidden, stack);

    public static Error TooManyRequests(string? code, string? message, IReadOnlyCollection<Error>? stack = null) =>
        new(code, message, ErrorType.TooManyRequests, stack);

    public static Error Exception(string? code, string? message, IReadOnlyCollection<Error>? stack = null) => new(code, message, ErrorType.Exception, stack);
}
