using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.Shared.Results;

public readonly record struct Result : IResult
{
    public ICollection<Error> Errors { get; init; }

    public Error FirstError => IsSuccess ? Error.None : Errors.First();

    public bool IsSuccess { get; init; }

    public bool IsFailure => !IsSuccess;

    public Result()
    {
        throw new InvalidOperationException("Use the static methods Success or Failure to instantiate Result.");
    }

    private Result(bool isSuccess, ICollection<Error> errors)
    {
        if (isSuccess && !errors.IsEmpty())
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && errors.IsEmpty())
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Errors = errors;
    }

    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }
     
        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Errors = [error];
    }

    public Result OnSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
        return this;
    }

    public Result OnFailure(Action action)
    {
        if (IsFailure)
        {
            action();
        }
        return this;
    }

    public static Result Success() => new(true, Error.EmptyErrors);

    public static Result Failure(Error error) => new(false, error);

    public static Result Failure(ICollection<Error> errors) => new(false, errors);

    public static Result Failure(Exception exception) => Failure(Error.Exception(exception.GetType().Name, exception.Message));

    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);

    public static Result<TValue> Failure<TValue>(Error error) => Result<TValue>.Failure(error);

    public static Result<TValue> Failure<TValue>(Exception exception) =>
        Result<TValue>.Failure(Error.Exception(exception.GetType().Name, exception.Message));

    public static Result<TValue> Failure<TValue>(ICollection<Error> errors) => Result<TValue>.Failure(errors);

    public static Result<TValue> Create<TValue>(TValue? value) => Result<TValue>.Create(value);
}
