using Enjoy.Domain.Shared.Results;

namespace Enjoy.Presentation.Common;

public static class ResultMatchAsyncExtensions
{
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Result<TValue>, TResult> onFailure)
    {
        if (result.IsFailure)
        {
            return onFailure(result);
        }

        return await onSuccess(result.Value);
    }
}
