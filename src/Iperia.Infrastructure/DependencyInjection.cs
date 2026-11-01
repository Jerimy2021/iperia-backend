using Iperia.Application.Ports.Out;
using Iperia.Infrastructure.Persistence;
using Iperia.Infrastructure.Persistence.Repositories;
using Iperia.Infrastructure.Security;
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
        var connectionString = configuration.GetConnectionString("IperiaDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Falta la cadena de conexión 'IperiaDb'.");
        }

        services.AddDbContext<IperiaDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
