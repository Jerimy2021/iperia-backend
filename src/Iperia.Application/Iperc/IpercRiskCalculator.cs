using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.Out;
using Iperia.Domain.Iperc;

namespace Iperia.Application.Iperc;

public sealed record RiskAssessment(int Probabilidad, int Severidad, int Valor, string Nivel);

/// <summary>
/// Calcula el riesgo R = P × S y su nivel usando la matriz IPERC configurada.
/// </summary>
public sealed class IpercRiskCalculator
{
    private readonly IIpercMatrixProvider _matrixProvider;

    public IpercRiskCalculator(IIpercMatrixProvider matrixProvider)
    {
        _matrixProvider = matrixProvider;
    }

    /// <param name="probabilidad">Nombre de un valor de la escala de probabilidad de la matriz.</param>
    /// <param name="severidad">Nombre de un valor de la escala de severidad de la matriz.</param>
    public RiskAssessment Calculate(string probabilidad, string severidad)
    {
        var matrix = _matrixProvider.GetMatrix();
        EnsureConfigured(matrix);

        var p = Lookup(matrix.Probabilidad, probabilidad, "probabilidad");
        var s = Lookup(matrix.Severidad, severidad, "severidad");
        var valor = Riesgo.Calcular(p, s);

        return new RiskAssessment(p, s, valor, ClassifyLevel(matrix, valor));
    }

    private static void EnsureConfigured(IpercMatrix matrix)
    {
        var missing = new List<string>();

        if (matrix.Severidad.Count == 0)
        {
            missing.Add("la escala de severidad");
        }

        if (matrix.Probabilidad.Count == 0)
        {
            missing.Add("la escala de probabilidad");
        }

        if (matrix.Niveles.Count == 0)
        {
            missing.Add("los umbrales de nivel de riesgo");
        }

        if (missing.Count > 0)
        {
            throw new IpercMatrixNotConfiguredException(
                $"La matriz IPERC no está completa: falta {string.Join(" y ", missing)} (config/iperc_matrix.json).");
        }
    }

    private static int Lookup(IReadOnlyDictionary<string, int> scale, string? name, string scaleName)
    {
        if (name is not null && scale.TryGetValue(name.Trim(), out var value))
        {
            return value;
        }

        throw new ValidationException(
            $"Valor de {scaleName} inválido. Valores permitidos: {string.Join(", ", scale.Keys)}.");
    }

    // El nivel es el último umbral cuyo "desde" no supera el valor de riesgo.
    private static string ClassifyLevel(IpercMatrix matrix, int valor)
    {
        var level = matrix.Niveles.LastOrDefault(n => n.Desde <= valor);

        return level?.Nivel
            ?? throw new IpercMatrixNotConfiguredException(
                $"Ningún umbral de la matriz IPERC cubre el valor de riesgo {valor}.");
    }
}
