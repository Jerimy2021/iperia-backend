using Iperia.Application.Ports.Out;

namespace Iperia.Tests.Fakes;

internal sealed class InMemoryEvidenceStorage : IEvidenceStorage
{
    public Dictionary<string, (byte[] Content, string ContentType)> Files { get; } = new();

    public async Task UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        Files[key] = (buffer.ToArray(), contentType);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        Files.Remove(key);
        return Task.CompletedTask;
    }
}
