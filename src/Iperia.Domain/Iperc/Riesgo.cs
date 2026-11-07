using Iperia.Domain.Common;
using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Iperc;

/// <summary>
/// Evaluación de un peligro: riesgo = probabilidad × severidad.
/// Los valores de las escalas y el nivel (ACEPTABLE, MODERADO...) los define la matriz IPERC configurada;
/// aquí solo vive la regla de cálculo.
/// </summary>
public sealed class Riesgo : Entity
{
    public int Probabilidad { get; private set; }
    public int Severidad { get; private set; }
    public int Valor { get; private set; }

    // Requerido por EF Core.
    private Riesgo()
    {
    }

    private Riesgo(Guid id, int probabilidad, int severidad) : base(id)
    {
        Probabilidad = probabilidad;
        Severidad = severidad;
        Valor = Calcular(probabilidad, severidad);
    }

    public static Riesgo Evaluar(int probabilidad, int severidad)
    {
        if (probabilidad < 0)
        {
            throw new DomainException("La probabilidad no puede ser negativa.");
        }

        if (severidad < 0)
        {
            throw new DomainException("La severidad no puede ser negativa.");
        }

        return new Riesgo(Guid.NewGuid(), probabilidad, severidad);
    }

    /// <summary>R = P × S.</summary>
    public static int Calcular(int probabilidad, int severidad) => checked(probabilidad * severidad);
}
