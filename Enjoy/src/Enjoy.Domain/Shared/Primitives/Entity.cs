namespace Enjoy.Domain.Shared.Primitives;

public abstract class Entity : IEquatable<Entity>
{
    public string Id { get; init; }

    protected Entity(string id) => Id = id;

    // Requerido para EF Core
    protected Entity() { }

    public override bool Equals(object? obj)
    {
        if (obj is Entity other)
        {
            return Equals(other);
        }

        return false;
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Id, other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode() * 41;

    public static bool operator ==(Entity? first, Entity? second) =>
        first is not null && first.Equals(second);

    public static bool operator !=(Entity? first, Entity? second) =>
        !(first == second);
}
