using Iperia.Domain.Common;
using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Evidences;

/// <summary>
/// Evidencia fotográfica: referencia al archivo guardado más sus metadatos (ubicación, fecha y responsable).
/// </summary>
public sealed class Evidence : AggregateRoot
{
    public const int StorageKeyMaxLength = 200;
    public const int ContentTypeMaxLength = 100;

    public string StorageKey { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public GeoLocation Location { get; private set; } = null!;
    public Guid UploadedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Requerido por EF Core.
    private Evidence()
    {
    }

    private Evidence(Guid id, string storageKey, string contentType, long sizeBytes, GeoLocation location,
        Guid uploadedByUserId, DateTime createdAt) : base(id)
    {
        StorageKey = storageKey;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        Location = location;
        UploadedByUserId = uploadedByUserId;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Registra una evidencia. La fecha-hora la pone el servidor, no el cliente.
    /// </summary>
    public static Evidence Register(Guid id, string storageKey, string contentType, long sizeBytes,
        GeoLocation location, Guid uploadedByUserId)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("La evidencia necesita un identificador.");
        }

        if (string.IsNullOrWhiteSpace(storageKey) || storageKey.Length > StorageKeyMaxLength)
        {
            throw new DomainException("La clave de almacenamiento no es válida.");
        }

        if (string.IsNullOrWhiteSpace(contentType) || contentType.Length > ContentTypeMaxLength)
        {
            throw new DomainException("El tipo de contenido no es válido.");
        }

        if (sizeBytes <= 0)
        {
            throw new DomainException("La evidencia no puede estar vacía.");
        }

        if (uploadedByUserId == Guid.Empty)
        {
            throw new DomainException("La evidencia necesita un responsable.");
        }

        return new Evidence(id, storageKey, contentType, sizeBytes, location, uploadedByUserId, DateTime.UtcNow);
    }
}
