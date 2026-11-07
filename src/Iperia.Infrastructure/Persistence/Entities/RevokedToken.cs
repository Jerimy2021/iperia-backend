namespace Iperia.Infrastructure.Persistence.Entities;

/// <summary>
/// Registro técnico de un token cerrado antes de expirar. No es un concepto del dominio.
/// </summary>
public class RevokedToken
{
    public string TokenId { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }

    private RevokedToken()
    {
    }

    public RevokedToken(string tokenId, DateTime expiresAt)
    {
        TokenId = tokenId;
        ExpiresAt = expiresAt;
    }
}
