namespace Iperia.Application.UseCases.Evidences;

/// <summary>
/// Límites de la subida de evidencias. Los valores vienen de la configuración (sección "Evidences").
/// </summary>
public sealed class EvidenceUploadOptions
{
    public const string SectionName = "Evidences";

    public int MaxSizeMegabytes { get; init; } = 10;

    public long MaxSizeBytes => MaxSizeMegabytes * 1024L * 1024L;

    public bool IsValid() => MaxSizeMegabytes > 0;
}
