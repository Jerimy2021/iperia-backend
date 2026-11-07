using Iperia.Domain.Evidences;

namespace Iperia.Application.Ports.In.Evidences;

public interface IUploadEvidenceUseCase
{
    Task<EvidenceResult> ExecuteAsync(UploadEvidenceCommand command, CancellationToken cancellationToken = default);
}

/// <param name="Content">Contenido del archivo; debe poder leerse desde el inicio.</param>
/// <param name="SizeBytes">Tamaño real del archivo recibido.</param>
/// <param name="UserId">Responsable: el usuario autenticado que sube la evidencia.</param>
public sealed record UploadEvidenceCommand(Stream Content, long SizeBytes, double Latitude, double Longitude, Guid UserId);

public sealed record EvidenceResult(
    Guid Id,
    string StorageKey,
    string ContentType,
    long SizeBytes,
    double Latitude,
    double Longitude,
    DateTime CreatedAt,
    Guid UploadedByUserId)
{
    public static EvidenceResult From(Evidence evidence) =>
        new(evidence.Id, evidence.StorageKey, evidence.ContentType, evidence.SizeBytes,
            evidence.Location.Latitude, evidence.Location.Longitude, evidence.CreatedAt, evidence.UploadedByUserId);
}
