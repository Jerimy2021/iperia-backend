using Amazon.S3;
using Amazon.S3.Model;
using Iperia.Application.Ports.Out;
using Microsoft.Extensions.Options;

namespace Iperia.Infrastructure.Storage;

/// <summary>
/// Adaptador de IEvidenceStorage sobre Amazon S3 (o cualquier servicio compatible, como MinIO en desarrollo).
/// </summary>
public class S3EvidenceStorage : IEvidenceStorage
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;

    public S3EvidenceStorage(IAmazonS3 s3, IOptions<S3Options> options)
    {
        _s3 = s3;
        _bucket = options.Value.BucketName;
    }

    public async Task UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        }, cancellationToken);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default) =>
        _s3.DeleteObjectAsync(_bucket, key, cancellationToken);
}
