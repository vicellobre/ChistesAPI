namespace Enjoy.Domain.Shared.Exceptions;

public sealed class MissingConnectionStringException(string connectionKeyName)
    : DomainException($"Connection string '{connectionKeyName}' not found. Set it in appsettings.json.");
