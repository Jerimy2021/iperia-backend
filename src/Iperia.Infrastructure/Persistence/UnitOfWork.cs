using Iperia.Application.Ports.Out;

namespace Iperia.Infrastructure.Persistence;

/// <summary>
/// Adaptador de IUnitOfWork sobre EF Core.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IperiaDbContext _context;

    public UnitOfWork(IperiaDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
