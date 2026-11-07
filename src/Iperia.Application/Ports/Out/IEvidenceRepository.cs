using Iperia.Domain.Evidences;

namespace Iperia.Application.Ports.Out;

public interface IEvidenceRepository
{
    Task AddAsync(Evidence evidence, CancellationToken cancellationToken = default);
}
