using Iperia.Application.Ports.Out;

namespace Iperia.Tests.Fakes;

internal sealed class InMemoryRevokedTokenStore : IRevokedTokenStore
{
    public Dictionary<string, DateTime> Revoked { get; } = new();

    public Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Revoked.ContainsKey(tokenId));

    public Task RevokeAsync(string tokenId, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        Revoked[tokenId] = expiresAt;
        return Task.CompletedTask;
    }
}
