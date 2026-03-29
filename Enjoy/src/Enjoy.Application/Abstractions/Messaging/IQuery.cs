using Enjoy.Domain.Shared.Results;
using MediatR;

namespace Enjoy.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
