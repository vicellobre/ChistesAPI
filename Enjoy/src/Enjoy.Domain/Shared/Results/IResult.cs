using Enjoy.Domain.Shared.Errors;

namespace Enjoy.Domain.Shared.Results;

public interface IResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    ICollection<Error> Errors { get; }

    Error FirstError { get; }
}

public interface IResult<out T> : IResult
{
    T Value { get; }
}
