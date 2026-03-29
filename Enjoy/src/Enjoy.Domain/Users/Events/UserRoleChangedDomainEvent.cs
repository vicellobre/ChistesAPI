using Enjoy.Domain.Shared.Messaging;

namespace Enjoy.Domain.Users.Events;

public sealed record UserRoleChangedDomainEvent(string Id, string UserId, string NewRole) : DomainEvent(Id);
