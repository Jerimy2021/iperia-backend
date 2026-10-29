namespace Iperia.Application.Common.Exceptions;

/// <summary>
/// Recurso no encontrado.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
