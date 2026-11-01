using Iperia.Application.Ports.Out;
using Iperia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Iperia.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra los adaptadores de salida (persistencia, seguridad, etc.).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IperiaDb")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'IperiaDb'.");

        services.AddDbContext<IperiaDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
