using System.Text.Json;
using Iperia.Application.Iperc;
using Iperia.Application.Ports.Out;

namespace Iperia.Infrastructure.Iperc;

/// <summary>
/// Lee la matriz IPERC de un archivo JSON. Vuelve a leerlo cuando el archivo cambia, así un cambio
/// de umbrales no requiere tocar código ni reiniciar.
/// </summary>
public class JsonIpercMatrixProvider : IIpercMatrixProvider
{
    public static readonly string[] RiskLevels = { "ACEPTABLE", "MODERADO", "IMPORTANTE", "INTOLERABLE" };

    private readonly string _path;
    private readonly object _lock = new();
    private DateTime _loadedWriteTimeUtc;
    private IpercMatrix? _cached;

    public JsonIpercMatrixProvider(string path)
    {
        _path = path;
    }

    public IpercMatrix GetMatrix()
    {
        if (!File.Exists(_path))
        {
            throw new InvalidOperationException($"No se encontró el archivo de la matriz IPERC: {_path}");
        }

        var writeTime = File.GetLastWriteTimeUtc(_path);

        lock (_lock)
        {
            if (_cached is null || writeTime != _loadedWriteTimeUtc)
            {
                _cached = Parse(File.ReadAllText(_path));
                _loadedWriteTimeUtc = writeTime;
            }

            return _cached;
        }
    }

    public static IpercMatrix Parse(string json)
    {
        using var document = JsonDocument.Parse(json, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
        var root = document.RootElement;

        var severidad = ReadScale(root, "severidad", scaleProperty: null);
        var probabilidad = ReadScale(root, "probabilidad", scaleProperty: "escala");
        var niveles = ReadLevels(root);

        return new IpercMatrix(severidad, probabilidad, niveles);
    }

    // Lee un objeto de pares nombre:valor ignorando las claves que empiezan con "_" (comentarios).
    private static Dictionary<string, int> ReadScale(JsonElement root, string section, string? scaleProperty)
    {
        var scale = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (!root.TryGetProperty(section, out var element))
        {
            return scale;
        }

        if (scaleProperty is not null && !element.TryGetProperty(scaleProperty, out element))
        {
            return scale;
        }

        foreach (var property in element.EnumerateObject().Where(p => !p.Name.StartsWith('_')))
        {
            if (property.Value.ValueKind != JsonValueKind.Number
                || !property.Value.TryGetInt32(out var value)
                || value < 0)
            {
                throw new InvalidOperationException(
                    $"Matriz IPERC inválida: '{section}.{property.Name}' debe ser un entero mayor o igual a 0.");
            }

            scale[property.Name] = value;
        }

        return scale;
    }

    private static List<RiskLevelThreshold> ReadLevels(JsonElement root)
    {
        var levels = new List<RiskLevelThreshold>();

        if (!root.TryGetProperty("umbrales", out var umbrales) || !umbrales.TryGetProperty("niveles", out var array))
        {
            return levels;
        }

        foreach (var item in array.EnumerateArray())
        {
            if (!item.TryGetProperty("nivel", out var nivel)
                || nivel.ValueKind != JsonValueKind.String
                || !item.TryGetProperty("desde", out var desde)
                || !desde.TryGetInt32(out var desdeValue))
            {
                throw new InvalidOperationException("Matriz IPERC inválida: cada nivel necesita 'nivel' (texto) y 'desde' (entero).");
            }

            levels.Add(new RiskLevelThreshold(nivel.GetString()!.Trim().ToUpperInvariant(), desdeValue));
        }

        if (levels.Count > 0)
        {
            Validate(levels);
        }

        return levels;
    }

    // Los umbrales completos son los 4 niveles, cada uno empezando más arriba que el anterior.
    private static void Validate(List<RiskLevelThreshold> levels)
    {
        if (!levels.Select(l => l.Nivel).SequenceEqual(RiskLevels))
        {
            throw new InvalidOperationException(
                $"Matriz IPERC inválida: los niveles deben ser, en este orden, {string.Join(", ", RiskLevels)}.");
        }

        for (var i = 1; i < levels.Count; i++)
        {
            if (levels[i].Desde <= levels[i - 1].Desde)
            {
                throw new InvalidOperationException(
                    $"Matriz IPERC inválida: 'desde' de {levels[i].Nivel} debe ser mayor que el de {levels[i - 1].Nivel}.");
            }
        }
    }
}
