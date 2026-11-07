using Iperia.Application.Ports.Out;
using Iperia.Domain.Evidences;

namespace Iperia.Infrastructure.Persistence.Repositories;

public class EvidenceRepository : IEvidenceRepository
{
    private readonly IperiaDbContext _context;

    public EvidenceRepository(IperiaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Evidence evidence, CancellationToken cancellationToken = default) =>
        await _context.Evidences.AddAsync(evidence, cancellationToken);
}
