namespace Iperia.Domain.Common;

/// <summary>
/// Base de toda entidad del dominio: se identifica por su Id, no por sus atributos.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    // Requerido por EF Core para materializar entidades.
    protected Entity()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other || obj.GetType() != GetType())
        {
            return false;
        }

        return Id != Guid.Empty && Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
