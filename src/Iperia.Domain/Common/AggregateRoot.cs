namespace Iperia.Domain.Common;

/// <summary>
/// Raíz de agregado: único punto de entrada para modificar el agregado
/// y la unidad que se persiste a través de los repositorios.
/// </summary>
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
