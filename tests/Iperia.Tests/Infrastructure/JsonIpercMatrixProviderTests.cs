using Iperia.Application.Common.Exceptions;
using Iperia.Application.Iperc;
using Iperia.Infrastructure.Iperc;

namespace Iperia.Tests.Infrastructure;

public class JsonIpercMatrixProviderTests
{
    // El archivo real del repo (config/iperc_matrix.json), copiado junto a los tests.
    private static string RealMatrixPath => Path.Combine(AppContext.BaseDirectory, "config", "iperc_matrix.json");

    [Fact]
    public void ArchivoReal_TraeLaEscalaDeSeveridadConfirmada()
    {
        var matrix = new JsonIpercMatrixProvider(RealMatrixPath).GetMatrix();

        Assert.Equal(0, matrix.Severidad["sinLesion"]);
        Assert.Equal(2, matrix.Severidad["lesionNoIncapacitante"]);
        Assert.Equal(4, matrix.Severidad["incapacitanteTemporal"]);
        Assert.Equal(6, matrix.Severidad["incapacitantePermanente"]);
        Assert.Equal(8, matrix.Severidad["fatalidad"]);
        Assert.Equal(5, matrix.Severidad.Count);
    }

    [Fact]
    public void ArchivoReal_NoInventaProbabilidadNiUmbralesPendientes()
    {
        var matrix = new JsonIpercMatrixProvider(RealMatrixPath).GetMatrix();

        // Pendiente de confirmar con Ing. Industrial: cuando se completen, este test debe actualizarse.
        Assert.Empty(matrix.Probabilidad);
        Assert.Empty(matrix.Niveles);
    }

    [Fact]
    public void ArchivoReal_ElCalculoAvisaQueLaMatrizNoEstaCompleta()
    {
        var calculator = new IpercRiskCalculator(new JsonIpercMatrixProvider(RealMatrixPath));

        var ex = Assert.Throws<IpercMatrixNotConfiguredException>(() => calculator.Calculate("alta", "fatalidad"));

        Assert.Contains("no está completa", ex.Message);
    }

    [Fact]
    public void SiElArchivoCambia_SeVuelveALeer()
    {
        var path = Path.Combine(Path.GetTempPath(), $"iperc_matrix_{Guid.NewGuid():N}.json");
        try
        {
            File.WriteAllText(path, """{ "severidad": { "fatalidad": 8 } }""");
            File.SetLastWriteTimeUtc(path, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            var provider = new JsonIpercMatrixProvider(path);
            Assert.Equal(8, provider.GetMatrix().Severidad["fatalidad"]);

            File.WriteAllText(path, """{ "severidad": { "fatalidad": 10 } }""");
            File.SetLastWriteTimeUtc(path, new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc));

            Assert.Equal(10, provider.GetMatrix().Severidad["fatalidad"]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ArchivoInexistente_LanzaInvalidOperation()
    {
        var provider = new JsonIpercMatrixProvider(Path.Combine(Path.GetTempPath(), "no-existe.json"));

        Assert.Throws<InvalidOperationException>(() => provider.GetMatrix());
    }

    [Theory]
    [InlineData("""{ "severidad": { "fatalidad": -1 } }""")]
    [InlineData("""{ "severidad": { "fatalidad": "ocho" } }""")]
    [InlineData("""{ "probabilidad": { "escala": { "alta": 1.5 } } }""")]
    [InlineData("""{ "umbrales": { "niveles": [ { "nivel": "ACEPTABLE" } ] } }""")]
    [InlineData("""{ "umbrales": { "niveles": [ { "nivel": "ACEPTABLE", "desde": 0 } ] } }""")]
    [InlineData("""{ "umbrales": { "niveles": [ { "nivel": "ACEPTABLE", "desde": 0 }, { "nivel": "MODERADO", "desde": 0 }, { "nivel": "IMPORTANTE", "desde": 5 }, { "nivel": "INTOLERABLE", "desde": 9 } ] } }""")]
    public void MatrizInvalida_LanzaInvalidOperation(string json)
    {
        Assert.Throws<InvalidOperationException>(() => JsonIpercMatrixProvider.Parse(json));
    }

    [Fact]
    public void LasClavesConGuionBajoSeIgnoran()
    {
        var matrix = JsonIpercMatrixProvider.Parse("""{ "severidad": { "_comentario": "texto", "fatalidad": 8 } }""");

        Assert.Single(matrix.Severidad);
    }
}
