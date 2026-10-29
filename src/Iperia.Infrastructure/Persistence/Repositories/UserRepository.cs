using Iperia.Application.Ports.Out;
using Iperia.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Iperia.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IperiaDbContext _context;

    public UserRepository(IperiaDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await _context.Users.AddAsync(user, cancellationToken);
}
