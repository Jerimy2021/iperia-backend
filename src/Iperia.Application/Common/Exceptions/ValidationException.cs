namespace Iperia.Application.Common.Exceptions;

/// <summary>
/// Datos de entrada inválidos para un caso de uso (ej. password débil).
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
