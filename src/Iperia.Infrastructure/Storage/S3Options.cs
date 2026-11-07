namespace Iperia.Infrastructure.Storage;

public sealed class S3Options
{
    public const string SectionName = "S3";

    public string BucketName { get; init; } = string.Empty;
    public string Region { get; init; } = "us-east-1";

    // Solo para desarrollo con MinIO. En AWS se dejan vacíos: se usa el rol IAM / variables de entorno.
    public string ServiceUrl { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;

    public bool UsesCustomEndpoint => !string.IsNullOrWhiteSpace(ServiceUrl);

    public bool IsValid() => !string.IsNullOrWhiteSpace(BucketName);
}
