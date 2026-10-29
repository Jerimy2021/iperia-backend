using Iperia.Application.Ports.Out;
using Iperia.Domain.Common;

namespace Iperia.Tests;

/// <summary>
/// Verifica que se respete la regla de dependencias de la arquitectura hexagonal:
/// las capas internas no conocen a las externas.
/// </summary>
public class ArchitectureTests
{
    private static IEnumerable<string> ReferencesOf(Type typeInAssembly) =>
        typeInAssembly.Assembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty);

    [Theory]
    [InlineData("Iperia.Application")]
    [InlineData("Iperia.Infrastructure")]
    [InlineData("Iperia.Api")]
    [InlineData("Microsoft.EntityFrameworkCore")]
    public void Domain_NoDependeDeCapasExternas(string forbidden)
    {
        Assert.DoesNotContain(forbidden, ReferencesOf(typeof(Entity)));
    }

    [Theory]
    [InlineData("Iperia.Infrastructure")]
    [InlineData("Iperia.Api")]
    [InlineData("Microsoft.EntityFrameworkCore")]
    public void Application_NoDependeDeInfraestructuraNiApi(string forbidden)
    {
        Assert.DoesNotContain(forbidden, ReferencesOf(typeof(IUnitOfWork)));
    }
}
