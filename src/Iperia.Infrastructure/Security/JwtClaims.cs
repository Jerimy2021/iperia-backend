namespace Iperia.Infrastructure.Security;

/// <summary>
/// Nombres de claims compartidos entre el generador de tokens y la validación en la Api.
/// </summary>
public static class JwtClaims
{
    public const string Subject = "sub";
    public const string Email = "email";
    public const string Name = "name";
    public const string Role = "role";
}
