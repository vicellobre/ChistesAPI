using MediatR;

namespace Enjoy.Domain.Shared.Messaging;

public interface IDomainEvent : INotification
{
    string Id { get; init; }
}
