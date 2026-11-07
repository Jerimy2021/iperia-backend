using Iperia.Domain.Exceptions;
using Iperia.Domain.Iperc;

namespace Iperia.Tests.Domain;

public class IpercModelTests
{
    [Theory]
    [InlineData(0, 8, 0)]
    [InlineData(1, 2, 2)]
    [InlineData(3, 6, 18)]
    [InlineData(5, 8, 40)]
    public void Riesgo_EsProbabilidadPorSeveridad(int probabilidad, int severidad, int esperado)
    {
        var riesgo = Riesgo.Evaluar(probabilidad, severidad);

        Assert.Equal(esperado, riesgo.Valor);
        Assert.Equal(probabilidad, riesgo.Probabilidad);
        Assert.Equal(severidad, riesgo.Severidad);
    }

    [Theory]
    [InlineData(-1, 2)]
    [InlineData(2, -1)]
    public void Riesgo_ConValoresNegativos_LanzaDomainException(int probabilidad, int severidad)
    {
        Assert.Throws<DomainException>(() => Riesgo.Evaluar(probabilidad, severidad));
    }

    [Fact]
    public void Puesto_ArmaLaJerarquiaHastaElRiesgo()
    {
        var puesto = Puesto.Crear("  Operador de torno  ");
        var tarea = puesto.AgregarTarea("Mecanizado de piezas");
        var actividad = tarea.AgregarActividad("Cambio de herramienta");
        var peligro = actividad.AgregarPeligro("Contacto con pieza giratoria");
        peligro.Evaluar(2, 6);

        Assert.Equal("Operador de torno", puesto.Nombre);
        Assert.Single(puesto.Tareas);
        Assert.Single(tarea.Actividades);
        Assert.Single(actividad.Peligros);
        Assert.Equal(12, peligro.Riesgo!.Valor);
    }

    [Fact]
    public void Peligro_ReevaluadoReemplazaElRiesgo()
    {
        var peligro = Puesto.Crear("P").AgregarTarea("T").AgregarActividad("A").AgregarPeligro("H");

        peligro.Evaluar(1, 2);
        peligro.Evaluar(3, 4);

        Assert.Equal(12, peligro.Riesgo!.Valor);
    }

    [Fact]
    public void Peligro_NuevoNoTieneRiesgoEvaluado()
    {
        var peligro = Puesto.Crear("P").AgregarTarea("T").AgregarActividad("A").AgregarPeligro("H");

        Assert.Null(peligro.Riesgo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NombresVacios_LanzanDomainException(string? nombre)
    {
        var puesto = Puesto.Crear("P");
        var tarea = puesto.AgregarTarea("T");
        var actividad = tarea.AgregarActividad("A");

        Assert.Throws<DomainException>(() => Puesto.Crear(nombre));
        Assert.Throws<DomainException>(() => puesto.AgregarTarea(nombre));
        Assert.Throws<DomainException>(() => tarea.AgregarActividad(nombre));
        Assert.Throws<DomainException>(() => actividad.AgregarPeligro(nombre));
    }

    [Fact]
    public void NombreMuyLargo_LanzaDomainException()
    {
        Assert.Throws<DomainException>(() => Puesto.Crear(new string('x', Puesto.NombreMaxLength + 1)));
    }
}
