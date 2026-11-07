using System.Security.Claims;
using Iperia.Api.Authentication;
using Iperia.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Iperia.Tests.Api;

public class PoliciesTests
{
    private static readonly IAuthorizationService Authorization = new ServiceCollection()
        .AddLogging()
        .AddAuthorization(options => options.AddRolePolicies())
        .BuildServiceProvider()
        .GetRequiredService<IAuthorizationService>();

    private static async Task<bool> IsAllowedAsync(Role role, string policy)
    {
        var identity = new ClaimsIdentity(new[] { new Claim("role", role.ToString()) }, "test", "name", "role");
        var result = await Authorization.AuthorizeAsync(new ClaimsPrincipal(identity), null, policy);
        return result.Succeeded;
    }

    [Theory]
    [InlineData(Role.Admin, true)]
    [InlineData(Role.Supervisor, true)]
    [InlineData(Role.Operario, false)]
    [InlineData(Role.Brigadista, false)]
    [InlineData(Role.GerenciaEhs, false)]
    public async Task AprobarOrdenDeTrabajo(Role role, bool allowed) =>
        Assert.Equal(allowed, await IsAllowedAsync(role, Policies.CanApproveWorkOrder));

    [Theory]
    [InlineData(Role.Admin, true)]
    [InlineData(Role.GerenciaEhs, true)]
    [InlineData(Role.Operario, false)]
    [InlineData(Role.Supervisor, false)]
    [InlineData(Role.Brigadista, false)]
    public async Task VerDashboardGerencial(Role role, bool allowed) =>
        Assert.Equal(allowed, await IsAllowedAsync(role, Policies.CanViewManagementDashboard));

    [Theory]
    [InlineData(nameof(Policies.AdminOnly))]
    [InlineData(nameof(Policies.AdminOrSupervisor))]
    [InlineData(nameof(Policies.CanApproveWorkOrder))]
    [InlineData(nameof(Policies.CanViewManagementDashboard))]
    public async Task Admin_TienePermisoEnTodasLasPoliticas(string policy) =>
        Assert.True(await IsAllowedAsync(Role.Admin, policy));

    [Theory]
    [InlineData(Role.Operario)]
    [InlineData(Role.Supervisor)]
    [InlineData(Role.GerenciaEhs)]
    [InlineData(Role.Brigadista)]
    public async Task SoloAdminGestionaUsuarios(Role role) =>
        Assert.False(await IsAllowedAsync(role, Policies.AdminOnly));
}
