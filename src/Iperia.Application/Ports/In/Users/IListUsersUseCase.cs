using Iperia.Application.Ports.In.Auth;

namespace Iperia.Application.Ports.In.Users;

public interface IListUsersUseCase
{
    Task<IReadOnlyList<UserResult>> ExecuteAsync(CancellationToken cancellationToken = default);
}
