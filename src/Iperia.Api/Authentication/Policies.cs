using Iperia.Domain.Users;
using Microsoft.AspNetCore.Authorization;

namespace Iperia.Api.Authentication;

/// <summary>
/// Políticas de autorización por rol. Se usan con [Authorize(Policy = Policies.X)].
/// Admin está incluido en todas: tiene los permisos de todos los roles.
/// </summary>
public static class Policies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string AdminOrSupervisor = nameof(AdminOrSupervisor);
    public const string CanApproveWorkOrder = nameof(CanApproveWorkOrder);
    public const string CanViewManagementDashboard = nameof(CanViewManagementDashboard);

    public static void AddRolePolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(AdminOnly, policy => policy.RequireRole(nameof(Role.Admin)));
        options.AddPolicy(AdminOrSupervisor, policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.Supervisor)));

        // Un Operario (o Brigadista) no puede aprobar órdenes de trabajo.
        options.AddPolicy(CanApproveWorkOrder, policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.Supervisor)));

        // El dashboard gerencial es solo para Gerencia EHS (y Admin).
        options.AddPolicy(CanViewManagementDashboard, policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.GerenciaEhs)));
    }
}
