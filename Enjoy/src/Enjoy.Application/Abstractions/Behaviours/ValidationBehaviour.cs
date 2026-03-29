using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;
using Enjoy.Domain.Shared.Results;
using FluentValidation;
using MediatR;

namespace Enjoy.Application.Abstractions.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        Error[] errors = _validators
            .Select(v => v.Validate(context))
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
        {
            return await next(cancellationToken);
        }

        return CreateValidationResult<TResponse>(errors);
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : IResult
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (TResult)(object)Result.Failure(errors);
        }

        var valueType = typeof(TResult).GenericTypeArguments[0];
        var result = typeof(Result<>)
            .MakeGenericType(valueType)
            .GetMethod(nameof(Result.Failure), [typeof(ICollection<Error>)])!
            .Invoke(null, [errors])!;

        return (TResult)result;
    }
}
