namespace Iperia.Application.Common.Exceptions;

/// <summary>
/// Credenciales inválidas o acceso no autorizado.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
