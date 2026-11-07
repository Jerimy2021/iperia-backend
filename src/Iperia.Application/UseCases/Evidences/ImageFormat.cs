namespace Iperia.Application.UseCases.Evidences;

/// <summary>
/// Formatos de imagen aceptados. Se detectan por los bytes iniciales del archivo:
/// la extensión y el Content-Type los controla el cliente y no son confiables.
/// </summary>
public sealed record ImageFormat(string ContentType, string Extension)
{
    /// <summary>Bytes necesarios para reconocer cualquiera de los formatos.</summary>
    public const int HeaderLength = 12;

    private static readonly ImageFormat Jpeg = new("image/jpeg", "jpg");
    private static readonly ImageFormat Png = new("image/png", "png");
    private static readonly ImageFormat Webp = new("image/webp", "webp");

    private static ReadOnlySpan<byte> PngSignature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    public static ImageFormat? Detect(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return Jpeg;
        }

        if (header.Length >= 8 && header[..8].SequenceEqual(PngSignature))
        {
            return Png;
        }

        // WebP: "RIFF" + tamaño (4 bytes) + "WEBP".
        if (header.Length >= 12
            && header[..4].SequenceEqual("RIFF"u8)
            && header.Slice(8, 4).SequenceEqual("WEBP"u8))
        {
            return Webp;
        }

        return null;
    }
}
