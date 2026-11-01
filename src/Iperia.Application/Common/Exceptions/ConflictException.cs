namespace Iperia.Application.Common.Exceptions;

/// <summary>
/// Conflicto con el estado actual (ej. email ya registrado).
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
