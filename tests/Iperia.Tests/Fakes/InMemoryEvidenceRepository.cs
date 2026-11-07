using Iperia.Application.Ports.Out;
using Iperia.Domain.Evidences;

namespace Iperia.Tests.Fakes;

internal sealed class InMemoryEvidenceRepository : IEvidenceRepository
{
    public List<Evidence> Evidences { get; } = new();

    public Task AddAsync(Evidence evidence, CancellationToken cancellationToken = default)
    {
        Evidences.Add(evidence);
        return Task.CompletedTask;
    }
}
