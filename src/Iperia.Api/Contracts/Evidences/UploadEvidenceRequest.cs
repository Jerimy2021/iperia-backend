using System.ComponentModel.DataAnnotations;

namespace Iperia.Api.Contracts.Evidences;

public sealed class UploadEvidenceRequest
{
    /// <summary>Imagen JPEG, PNG o WebP dentro del límite configurado (por defecto 10 MB).</summary>
    [Required]
    public IFormFile File { get; init; } = null!;

    /// <summary>Latitud GPS en grados decimales (-90 a 90).</summary>
    [Required]
    public double? Latitude { get; init; }

    /// <summary>Longitud GPS en grados decimales (-180 a 180).</summary>
    [Required]
    public double? Longitude { get; init; }
}
