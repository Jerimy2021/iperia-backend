namespace Iperia.Application.Ports.Out;

/// <summary>
/// Puerto de salida para invalidar tokens antes de su expiración (cierre de sesión).
/// </summary>
public interface IRevokedTokenStore
{
    Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca el token como revocado hasta <paramref name="expiresAt"/>, momento en que ya no hace falta recordarlo.
    /// </summary>
    Task RevokeAsync(string tokenId, DateTime expiresAt, CancellationToken cancellationToken = default);
}
