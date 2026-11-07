using Iperia.Domain.Common;
using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Iperc;

public sealed class Peligro : Entity
{
    public const int DescripcionMaxLength = 500;

    public string Descripcion { get; private set; } = null!;
    public Riesgo? Riesgo { get; private set; }

    // Requerido por EF Core.
    private Peligro()
    {
    }

    internal Peligro(string? descripcion) : base(Guid.NewGuid())
    {
        Descripcion = Texto.Validar(descripcion, DescripcionMaxLength, "La descripción del peligro");
    }

    /// <summary>
    /// Registra (o reemplaza) la evaluación del riesgo de este peligro.
    /// </summary>
    public Riesgo Evaluar(int probabilidad, int severidad)
    {
        Riesgo = Riesgo.Evaluar(probabilidad, severidad);
        return Riesgo;
    }
}
