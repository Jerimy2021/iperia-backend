using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Iperc;

/// <summary>
/// Validación común de los textos obligatorios del modelo IPERC.
/// </summary>
internal static class Texto
{
    public static string Validar(string? valor, int maxLength, string campo)
    {
        var texto = valor?.Trim() ?? string.Empty;

        if (texto.Length == 0)
        {
            throw new DomainException($"{campo} es obligatorio.");
        }

        if (texto.Length > maxLength)
        {
            throw new DomainException($"{campo} no puede superar {maxLength} caracteres.");
        }

        return texto;
    }
}
