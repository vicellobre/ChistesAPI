using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Extensions;

namespace Enjoy.Domain.Shared.Results;

public readonly record struct Result<TValue> : IResult<TValue>
{
    private readonly TValue? _value;
    
    public ICollection<Error> Errors { get; init; }

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public bool IsSuccess => Errors.IsEmpty();

    public bool IsFailure => !IsSuccess;

    public Error FirstError => IsFailure ? Errors.First() : Error.None;

    public Result()
    {
        throw new InvalidOperationException("Use the static methods Success, Failure or Create to instantiate Result.");
    }

    private Result(TValue value)
    {
        Errors = value is not null ? Error.EmptyErrors : [Error.NullValue];
        _value = value;
    }

    private Result(Error error)
    {
        Errors = !error.Equals(Error.None) ? [error] : [Error.NullValue];
    }

    private Result(ICollection<Error> errors)
    {
        Errors = !errors.IsNullOrEmpty() ? errors : [Error.NullValue];
    }

    public Result<TValue> OnSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
        return this;
    }

    public Result<TValue> OnFailure(Action action)
    {
        if (IsFailure)
        {
            action();
        }
        return this;
    }

    public TValue? GetValueOrDefault(TValue? defaultValue = default) => IsSuccess ? _value : defaultValue;

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<TValue> Failure(Error error) => new(error);

    public static Result<TValue> Failure(Exception exception) =>
        Failure(Error.Exception(exception.GetType().Name, exception.Message));

    public static Result<TValue> Failure(ICollection<Error> errors) => new(errors);

    public static Result<TValue> Create(TValue? value) => value is not null ? Success(value) : Failure(Error.NullValue);

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Error error) => new(error);

    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

    public static implicit operator Result<TValue>(HashSet<Error> errors) => new(errors);

    public static implicit operator Result<TValue>(Error[] errors) => new(errors);

    public static implicit operator Result(Result<TValue> result) => result.IsSuccess ? Result.Success() : Result.Failure(result.Errors);
}
