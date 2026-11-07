using Iperia.Domain.Common;

namespace Iperia.Domain.Iperc;

public sealed class Tarea : Entity
{
    public const int NombreMaxLength = 200;

    private readonly List<Actividad> _actividades = new();

    public string Nombre { get; private set; } = null!;
    public IReadOnlyList<Actividad> Actividades => _actividades;

    // Requerido por EF Core.
    private Tarea()
    {
    }

    internal Tarea(string? nombre) : base(Guid.NewGuid())
    {
        Nombre = Texto.Validar(nombre, NombreMaxLength, "El nombre de la tarea");
    }

    public Actividad AgregarActividad(string? nombre)
    {
        var actividad = new Actividad(nombre);
        _actividades.Add(actividad);
        return actividad;
    }
}
