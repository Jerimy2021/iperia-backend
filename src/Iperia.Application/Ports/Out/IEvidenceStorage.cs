namespace Iperia.Application.Ports.Out;

/// <summary>
/// Puerto de salida para guardar los archivos de evidencia (S3 en producción).
/// </summary>
public interface IEvidenceStorage
{
    Task UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
