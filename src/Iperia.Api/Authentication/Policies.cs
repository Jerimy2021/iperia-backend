using Iperia.Domain.Users;
using Microsoft.AspNetCore.Authorization;

namespace Iperia.Api.Authentication;

/// <summary>
/// Políticas de autorización por rol. Se usan con [Authorize(Policy = Policies.X)].
/// </summary>
public static class Policies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string AdminOrSupervisor = nameof(AdminOrSupervisor);

    public static void AddRolePolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(AdminOnly, policy => policy.RequireRole(nameof(Role.Admin)));
        options.AddPolicy(AdminOrSupervisor, policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.Supervisor)));
    }
}
