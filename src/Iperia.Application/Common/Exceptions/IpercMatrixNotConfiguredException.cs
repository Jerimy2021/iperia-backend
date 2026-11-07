namespace Iperia.Application.Common.Exceptions;

/// <summary>
/// La matriz IPERC todavía no tiene todos los valores necesarios (escala de probabilidad o umbrales).
/// </summary>
public class IpercMatrixNotConfiguredException : Exception
{
    public IpercMatrixNotConfiguredException(string message) : base(message)
    {
    }
}
