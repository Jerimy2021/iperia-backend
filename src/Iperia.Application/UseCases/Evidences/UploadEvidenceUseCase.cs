using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Evidences;
using Iperia.Application.Ports.Out;
using Iperia.Domain.Evidences;

namespace Iperia.Application.UseCases.Evidences;

public sealed class UploadEvidenceUseCase : IUploadEvidenceUseCase
{
    private readonly IEvidenceStorage _storage;
    private readonly IEvidenceRepository _evidences;
    private readonly IUnitOfWork _unitOfWork;

    public UploadEvidenceUseCase(IEvidenceStorage storage, IEvidenceRepository evidences, IUnitOfWork unitOfWork)
    {
        _storage = storage;
        _evidences = evidences;
        _unitOfWork = unitOfWork;
    }

    public async Task<EvidenceResult> ExecuteAsync(UploadEvidenceCommand command, CancellationToken cancellationToken = default)
    {
        if (command.SizeBytes <= 0)
        {
            throw new ValidationException("El archivo está vacío.");
        }

        if (command.SizeBytes > ImageFormat.MaxSizeBytes)
        {
            throw new ValidationException(
                $"El archivo supera el tamaño máximo de {ImageFormat.MaxSizeBytes / (1024 * 1024)} MB.");
        }

        var format = await DetectFormatAsync(command.Content, cancellationToken)
            ?? throw new ValidationException("El archivo no es una imagen válida. Formatos permitidos: JPEG, PNG y WebP.");

        // Valida las coordenadas antes de subir nada a S3.
        var location = GeoLocation.Create(command.Latitude, command.Longitude);

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var key = $"evidencias/{now:yyyy}/{now:MM}/{id}.{format.Extension}";

        await _storage.UploadAsync(key, command.Content, format.ContentType, cancellationToken);

        try
        {
            var evidence = Evidence.Register(id, key, format.ContentType, command.SizeBytes, location, command.UserId);
            await _evidences.AddAsync(evidence, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return EvidenceResult.From(evidence);
        }
        catch
        {
            // No dejar archivos huérfanos en S3 si no se pudo registrar la evidencia.
            await _storage.DeleteAsync(key, CancellationToken.None);
            throw;
        }
    }

    private static async Task<ImageFormat?> DetectFormatAsync(Stream content, CancellationToken cancellationToken)
    {
        if (!content.CanSeek)
        {
            throw new InvalidOperationException("El contenido de la evidencia debe poder leerse desde el inicio.");
        }

        var header = new byte[ImageFormat.HeaderLength];
        content.Position = 0;
        var read = await content.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, cancellationToken);
        content.Position = 0;

        return ImageFormat.Detect(header.AsSpan(0, read));
    }
}
