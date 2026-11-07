namespace Iperia.Application.Iperc;

/// <summary>Nivel de riesgo que empieza en el valor <paramref name="Desde"/> (inclusive) y llega hasta el siguiente umbral.</summary>
public sealed record RiskLevelThreshold(string Nivel, int Desde);

/// <summary>
/// Matriz IPERC cargada desde la configuración (config/iperc_matrix.json): escalas de severidad y probabilidad
/// y umbrales de nivel. Nada de esto se define en código.
/// </summary>
public sealed class IpercMatrix
{
    public IReadOnlyDictionary<string, int> Severidad { get; }
    public IReadOnlyDictionary<string, int> Probabilidad { get; }

    /// <summary>Umbrales ordenados de menor a mayor.</summary>
    public IReadOnlyList<RiskLevelThreshold> Niveles { get; }

    public IpercMatrix(
        IReadOnlyDictionary<string, int> severidad,
        IReadOnlyDictionary<string, int> probabilidad,
        IReadOnlyList<RiskLevelThreshold> niveles)
    {
        Severidad = new Dictionary<string, int>(severidad, StringComparer.OrdinalIgnoreCase);
        Probabilidad = new Dictionary<string, int>(probabilidad, StringComparer.OrdinalIgnoreCase);
        Niveles = niveles;
    }
}
