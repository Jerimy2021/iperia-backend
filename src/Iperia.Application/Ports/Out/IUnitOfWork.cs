namespace Iperia.Application.Ports.Out;

/// <summary>
/// Puerto de salida para confirmar los cambios de un caso de uso en una sola transacción.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
