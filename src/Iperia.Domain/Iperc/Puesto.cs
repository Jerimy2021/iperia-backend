using Iperia.Domain.Common;

namespace Iperia.Domain.Iperc;

/// <summary>
/// Puesto de trabajo: raíz de la matriz IPERC. Puesto → Tarea → Actividad → Peligro → Riesgo.
/// </summary>
public sealed class Puesto : AggregateRoot
{
    public const int NombreMaxLength = 150;

    private readonly List<Tarea> _tareas = new();

    public string Nombre { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<Tarea> Tareas => _tareas;

    // Requerido por EF Core.
    private Puesto()
    {
    }

    private Puesto(Guid id, string nombre, DateTime createdAt) : base(id)
    {
        Nombre = nombre;
        CreatedAt = createdAt;
    }

    public static Puesto Crear(string? nombre) =>
        new(Guid.NewGuid(), Texto.Validar(nombre, NombreMaxLength, "El nombre del puesto"), DateTime.UtcNow);

    public Tarea AgregarTarea(string? nombre)
    {
        var tarea = new Tarea(nombre);
        _tareas.Add(tarea);
        return tarea;
    }
}
