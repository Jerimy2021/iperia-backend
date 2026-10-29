using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.In.Users;
using Iperia.Application.Ports.Out;

namespace Iperia.Application.UseCases.Users;

public sealed class ListUsersUseCase : IListUsersUseCase
{
    private readonly IUserRepository _users;

    public ListUsersUseCase(IUserRepository users)
    {
        _users = users;
    }

    public async Task<IReadOnlyList<UserResult>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _users.ListAsync(cancellationToken);

        return users.Select(UserResult.From).ToList();
    }
}
