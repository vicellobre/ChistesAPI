using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.Shared.Results;

public static class ResultExtensions
{
    public static Result<TValue> ToResult<TValue>(this TValue value) => value;

    public static Result<TValue> ToResult<TValue>(this Error error) => error;

    public static Result<TValue> ToResult<TValue>(this ICollection<Error> errors) => errors.ToList();

    public static Result<TValue> ToResult<TValue>(this List<Error> errors) => errors;

    public static Result<TValue> ToResult<TValue>(this HashSet<Error> errors) => errors;

    public static Result<TValue> ToResult<TValue>(this Error[] errors) => errors;

    public static TResult Match<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.FirstError);

    public static TResult Match<TResult>(this Result result, Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.FirstError);
}
