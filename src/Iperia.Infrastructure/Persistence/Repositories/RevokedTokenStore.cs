using Iperia.Application.Ports.Out;
using Iperia.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Iperia.Infrastructure.Persistence.Repositories;

public class RevokedTokenStore : IRevokedTokenStore
{
    private readonly IperiaDbContext _context;

    public RevokedTokenStore(IperiaDbContext context)
    {
        _context = context;
    }

    public Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken = default) =>
        _context.RevokedTokens.AnyAsync(t => t.TokenId == tokenId, cancellationToken);

    public async Task RevokeAsync(string tokenId, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        // Un token expirado ya es rechazado por la validación de vida útil: se limpia para que la tabla no crezca sin límite.
        await _context.RevokedTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.RevokedTokens.AddAsync(new RevokedToken(tokenId, expiresAt), cancellationToken);
    }
}
