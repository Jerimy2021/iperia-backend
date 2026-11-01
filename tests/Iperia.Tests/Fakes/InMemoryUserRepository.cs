using Iperia.Application.Ports.Out;
using Iperia.Domain.Users;

namespace Iperia.Tests.Fakes;

internal sealed class InMemoryUserRepository : IUserRepository
{
    public List<User> Users { get; } = new();

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        Task.FromResult(Users.Any(u => u.Email == email));

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<User>>(Users.ToList());

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        Users.Add(user);
        return Task.CompletedTask;
    }
}
