using Enjoy.Domain.Shared.Results;
using MediatR;

namespace Enjoy.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
