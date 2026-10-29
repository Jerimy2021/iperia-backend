using Iperia.Application.Ports.Out;
using Iperia.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Iperia.Infrastructure.Persistence.Seed;

/// <summary>
/// Crea el primer Admin a partir de la sección "SeedAdmin" de la configuración.
/// Sin un Admin inicial nadie podría asignar roles.
/// </summary>
public static class AdminSeeder
{
    public const string SectionName = "SeedAdmin";

    public static async Task SeedAdminAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var section = provider.GetRequiredService<IConfiguration>().GetSection(SectionName);
        var fullName = section["FullName"];
        var email = section["Email"];
        var password = section["Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var users = provider.GetRequiredService<IUserRepository>();
        var adminEmail = Email.Create(email);

        if (await users.ExistsByEmailAsync(adminEmail, cancellationToken))
        {
            return;
        }

        var hasher = provider.GetRequiredService<IPasswordHasher>();
        var admin = User.Register(fullName ?? "Administrador", adminEmail, hasher.Hash(password), Role.Admin);

        await users.AddAsync(admin, cancellationToken);
        await provider.GetRequiredService<IUnitOfWork>().SaveChangesAsync(cancellationToken);

        provider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(AdminSeeder))
            .LogInformation("Admin inicial creado: {Email}", adminEmail.Value);
    }
}
