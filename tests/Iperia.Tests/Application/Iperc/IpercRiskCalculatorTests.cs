using Iperia.Application.Common.Exceptions;
using Iperia.Application.Iperc;
using Iperia.Application.Ports.Out;
using Iperia.Infrastructure.Iperc;

namespace Iperia.Tests.Application.Iperc;

public class IpercRiskCalculatorTests
{
    // Matriz de PRUEBA: los valores de probabilidad y los umbrales NO son los reales (están pendientes de
    // confirmar con Ing. Industrial). Los resultados esperados se derivan de estos mismos datos.
    private const string SampleMatrix = """
        {
          "severidad": { "sinLesion": 0, "lesionNoIncapacitante": 2, "incapacitanteTemporal": 4, "incapacitantePermanente": 6, "fatalidad": 8 },
          "probabilidad": { "escala": { "baja": 1, "media": 2, "alta": 3 } },
          "umbrales": { "niveles": [
            { "nivel": "ACEPTABLE", "desde": 0 },
            { "nivel": "MODERADO", "desde": 5 },
            { "nivel": "IMPORTANTE", "desde": 10 },
            { "nivel": "INTOLERABLE", "desde": 20 }
          ] }
        }
        """;

    private static IpercRiskCalculator CalculatorFor(string json) =>
        new(new FixedMatrixProvider(JsonIpercMatrixProvider.Parse(json)));

    [Fact]
    public void ElRiesgoEsProbabilidadPorSeveridad_ParaTodaLaMatriz()
    {
        var matrix = JsonIpercMatrixProvider.Parse(SampleMatrix);
        var calculator = new IpercRiskCalculator(new FixedMatrixProvider(matrix));

        foreach (var (probabilidadNombre, p) in matrix.Probabilidad)
        {
            foreach (var (severidadNombre, s) in matrix.Severidad)
            {
                var result = calculator.Calculate(probabilidadNombre, severidadNombre);

                Assert.Equal(p, result.Probabilidad);
                Assert.Equal(s, result.Severidad);
                Assert.Equal(p * s, result.Valor);
            }
        }
    }

    [Theory]
    [InlineData("baja", "lesionNoIncapacitante", 2, "ACEPTABLE")]
    [InlineData("media", "incapacitanteTemporal", 8, "MODERADO")]
    [InlineData("alta", "incapacitanteTemporal", 12, "IMPORTANTE")]
    [InlineData("alta", "fatalidad", 24, "INTOLERABLE")]
    [InlineData("alta", "sinLesion", 0, "ACEPTABLE")]
    public void ClasificaElNivelSegunLosUmbralesDeLaMatriz(string probabilidad, string severidad, int valor, string nivel)
    {
        var result = CalculatorFor(SampleMatrix).Calculate(probabilidad, severidad);

        Assert.Equal(valor, result.Valor);
        Assert.Equal(nivel, result.Nivel);
    }

    [Fact]
    public void UnValorExactamenteEnElUmbralPerteneceAEseNivel()
    {
        // Se mueve el umbral de MODERADO a 8 para que R = 8 caiga justo en el límite.
        var json = SampleMatrix.Replace("\"MODERADO\", \"desde\": 5", "\"MODERADO\", \"desde\": 8");

        var result = CalculatorFor(json).Calculate("media", "incapacitanteTemporal"); // R = 8

        Assert.Equal("MODERADO", result.Nivel);
    }

    [Fact]
    public void CambiarUnUmbralEnElJson_CambiaElResultadoSinTocarCodigo()
    {
        const string probabilidad = "media";
        const string severidad = "incapacitanteTemporal"; // R = 8

        var antes = CalculatorFor(SampleMatrix).Calculate(probabilidad, severidad);
        var cambiada = SampleMatrix.Replace("\"IMPORTANTE\", \"desde\": 10", "\"IMPORTANTE\", \"desde\": 7");
        var despues = CalculatorFor(cambiada).Calculate(probabilidad, severidad);

        Assert.Equal(antes.Valor, despues.Valor);
        Assert.Equal("MODERADO", antes.Nivel);
        Assert.Equal("IMPORTANTE", despues.Nivel);
    }

    [Fact]
    public void LosNombresDeLaEscalaNoDistinguenMayusculas()
    {
        var result = CalculatorFor(SampleMatrix).Calculate(" ALTA ", "Fatalidad");

        Assert.Equal(24, result.Valor);
    }

    [Theory]
    [InlineData("muyAlta", "fatalidad")]
    [InlineData("alta", "catastrofe")]
    [InlineData("", "fatalidad")]
    [InlineData(null, "fatalidad")]
    public void ValoresFueraDeLaEscala_LanzanValidationConLosPermitidos(string? probabilidad, string severidad)
    {
        var ex = Assert.Throws<ValidationException>(() => CalculatorFor(SampleMatrix).Calculate(probabilidad!, severidad));

        Assert.Contains("Valores permitidos", ex.Message);
    }

    [Fact]
    public void SinEscalaDeProbabilidad_LaMatrizNoEstaCompleta()
    {
        var json = SampleMatrix.Replace("{ \"baja\": 1, \"media\": 2, \"alta\": 3 }", "{}");

        var ex = Assert.Throws<IpercMatrixNotConfiguredException>(() => CalculatorFor(json).Calculate("alta", "fatalidad"));

        Assert.Contains("escala de probabilidad", ex.Message);
    }

    private sealed class FixedMatrixProvider : IIpercMatrixProvider
    {
        private readonly IpercMatrix _matrix;

        public FixedMatrixProvider(IpercMatrix matrix)
        {
            _matrix = matrix;
        }

        public IpercMatrix GetMatrix() => _matrix;
    }
}
