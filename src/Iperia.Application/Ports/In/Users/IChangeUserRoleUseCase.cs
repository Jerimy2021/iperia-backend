using Iperia.Application.Ports.In.Auth;

namespace Iperia.Application.Ports.In.Users;

public interface IChangeUserRoleUseCase
{
    Task<UserResult> ExecuteAsync(ChangeUserRoleCommand command, CancellationToken cancellationToken = default);
}

/// <param name="RequestedBy">Id del Admin que hace el cambio (sale del token).</param>
public sealed record ChangeUserRoleCommand(Guid UserId, string Role, Guid RequestedBy);
