using Iperia.Api.Authentication;
using Iperia.Api.Contracts.Users;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.In.Users;
using Iperia.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iperia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class UsersController : ControllerBase
{
    private readonly IListUsersUseCase _listUsers;
    private readonly IChangeUserRoleUseCase _changeUserRole;

    public UsersController(IListUsersUseCase listUsers, IChangeUserRoleUseCase changeUserRole)
    {
        _listUsers = listUsers;
        _changeUserRole = changeUserRole;
    }

    /// <summary>
    /// Lista todos los usuarios. Solo Admin y Supervisor.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Policies.AdminOrSupervisor)]
    [ProducesResponseType(typeof(IReadOnlyList<UserResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken) =>
        Ok(await _listUsers.ExecuteAsync(cancellationToken));

    /// <summary>
    /// Cambia el rol de un usuario (Admin, Supervisor, Worker). Solo Admin.
    /// </summary>
    [HttpPatch("{id:guid}/role")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(UserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(Guid id, ChangeRoleRequest request, CancellationToken cancellationToken)
    {
        var requestedBy = Guid.Parse(User.FindFirst(JwtClaims.Subject)!.Value);

        var user = await _changeUserRole.ExecuteAsync(
            new ChangeUserRoleCommand(id, request.Role, requestedBy),
            cancellationToken);

        return Ok(user);
    }
}
