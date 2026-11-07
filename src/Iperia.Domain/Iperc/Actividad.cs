using Iperia.Domain.Common;

namespace Iperia.Domain.Iperc;

public sealed class Actividad : Entity
{
    public const int NombreMaxLength = 200;

    private readonly List<Peligro> _peligros = new();

    public string Nombre { get; private set; } = null!;
    public IReadOnlyList<Peligro> Peligros => _peligros;

    // Requerido por EF Core.
    private Actividad()
    {
    }

    internal Actividad(string? nombre) : base(Guid.NewGuid())
    {
        Nombre = Texto.Validar(nombre, NombreMaxLength, "El nombre de la actividad");
    }

    public Peligro AgregarPeligro(string? descripcion)
    {
        var peligro = new Peligro(descripcion);
        _peligros.Add(peligro);
        return peligro;
    }
}
