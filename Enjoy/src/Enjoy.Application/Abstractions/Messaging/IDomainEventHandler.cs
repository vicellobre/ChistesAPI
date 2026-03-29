using Enjoy.Domain.Shared.Messaging;
using MediatR;

namespace Enjoy.Application.Abstractions.Messaging;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent;
