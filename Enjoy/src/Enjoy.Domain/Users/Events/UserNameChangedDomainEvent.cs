using Enjoy.Domain.Shared.Messaging;

namespace Enjoy.Domain.Users.Events;

public sealed record UserNameChangedDomainEvent(string Id, string UserId) : DomainEvent(Id);
