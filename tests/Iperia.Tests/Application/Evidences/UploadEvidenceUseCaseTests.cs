using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Evidences;
using Iperia.Application.Ports.Out;
using Iperia.Application.UseCases.Evidences;
using Iperia.Domain.Evidences;
using Iperia.Domain.Exceptions;
using Iperia.Tests.Fakes;

namespace Iperia.Tests.Application.Evidences;

public class UploadEvidenceUseCaseTests
{
    private static readonly byte[] Jpeg = { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
    private static readonly byte[] Png = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D };
    private static readonly byte[] Webp = "RIFF\0\0\0\0WEBPVP8 "u8.ToArray();

    private readonly InMemoryEvidenceStorage _storage = new();
    private readonly InMemoryEvidenceRepository _evidences = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly Guid _userId = Guid.NewGuid();

    private readonly EvidenceUploadOptions _options = new();

    private UploadEvidenceUseCase CreateUseCase(IUnitOfWork? unitOfWork = null, EvidenceUploadOptions? options = null) =>
        new(_storage, _evidences, unitOfWork ?? _unitOfWork, options ?? _options);

    private UploadEvidenceCommand Command(byte[] content, double lat = -12.0464, double lon = -77.0428) =>
        new(new MemoryStream(content), content.Length, lat, lon, _userId);

    [Theory]
    [InlineData("jpeg", "image/jpeg", ".jpg")]
    [InlineData("png", "image/png", ".png")]
    [InlineData("webp", "image/webp", ".webp")]
    public async Task ImagenValida_SeGuardaEnStorageConMetadatos(string format, string contentType, string extension)
    {
        var content = format switch { "jpeg" => Jpeg, "png" => Png, _ => Webp };
        var before = DateTime.UtcNow;

        var result = await CreateUseCase().ExecuteAsync(Command(content));

        Assert.Equal(contentType, result.ContentType);
        Assert.EndsWith(extension, result.StorageKey);
        Assert.Contains(result.Id.ToString(), result.StorageKey);
        Assert.Equal(content, _storage.Files[result.StorageKey].Content);
        Assert.Equal(contentType, _storage.Files[result.StorageKey].ContentType);

        var evidence = Assert.Single(_evidences.Evidences);
        Assert.Equal(result.Id, evidence.Id);
        Assert.Equal(_userId, evidence.UploadedByUserId);
        Assert.Equal(-12.0464, evidence.Location.Latitude);
        Assert.Equal(-77.0428, evidence.Location.Longitude);
        Assert.InRange(evidence.CreatedAt, before, DateTime.UtcNow);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CadaSubida_GeneraUnIdUnico()
    {
        var useCase = CreateUseCase();

        var first = await useCase.ExecuteAsync(Command(Jpeg));
        var second = await useCase.ExecuteAsync(Command(Jpeg));

        Assert.NotEqual(first.Id, second.Id);
        Assert.NotEqual(first.StorageKey, second.StorageKey);
    }

    [Fact]
    public async Task ArchivoDeTextoConExtensionDeImagen_SeRechaza()
    {
        var text = "esto no es una imagen"u8.ToArray();

        await Assert.ThrowsAsync<ValidationException>(() => CreateUseCase().ExecuteAsync(Command(text)));

        Assert.Empty(_storage.Files);
        Assert.Empty(_evidences.Evidences);
    }

    [Fact]
    public async Task ArchivoMasPequenoQueLaCabecera_SeRechaza()
    {
        await Assert.ThrowsAsync<ValidationException>(() => CreateUseCase().ExecuteAsync(Command(new byte[] { 0xFF })));
    }

    [Fact]
    public async Task ArchivoVacio_SeRechaza()
    {
        var command = new UploadEvidenceCommand(new MemoryStream(), 0, 0, 0, _userId);

        await Assert.ThrowsAsync<ValidationException>(() => CreateUseCase().ExecuteAsync(command));
    }

    [Fact]
    public async Task ArchivoMayorAlLimite_SeRechazaSinSubirNada()
    {
        var command = new UploadEvidenceCommand(new MemoryStream(Jpeg), _options.MaxSizeBytes + 1, 0, 0, _userId);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => CreateUseCase().ExecuteAsync(command));

        Assert.Contains("10 MB", ex.Message);
        Assert.Empty(_storage.Files);
    }

    [Fact]
    public async Task ArchivoExactamenteEnElLimite_SeAcepta()
    {
        var command = new UploadEvidenceCommand(new MemoryStream(Jpeg), _options.MaxSizeBytes, 0, 0, _userId);

        var result = await CreateUseCase().ExecuteAsync(command);

        Assert.Equal(_options.MaxSizeBytes, result.SizeBytes);
    }

    [Fact]
    public async Task ElLimiteSeTomaDeLaConfiguracion()
    {
        var useCase = CreateUseCase(options: new EvidenceUploadOptions { MaxSizeMegabytes = 1 });
        var command = new UploadEvidenceCommand(new MemoryStream(Jpeg), 1024 * 1024 + 1, 0, 0, _userId);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecuteAsync(command));

        Assert.Contains("1 MB", ex.Message);
    }

    [Fact]
    public void ElLimitePorDefectoEs10Mb()
    {
        Assert.Equal(10 * 1024 * 1024, new EvidenceUploadOptions().MaxSizeBytes);
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(0, -181)]
    public async Task CoordenadasInvalidas_SeRechazanSinSubirNada(double lat, double lon)
    {
        await Assert.ThrowsAsync<DomainException>(() => CreateUseCase().ExecuteAsync(Command(Jpeg, lat, lon)));

        Assert.Empty(_storage.Files);
        Assert.Empty(_evidences.Evidences);
    }

    [Fact]
    public async Task SiFallaElGuardadoEnBd_BorraElArchivoDeStorage()
    {
        var useCase = CreateUseCase(new FailingUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(Command(Jpeg)));

        Assert.Empty(_storage.Files);
    }

    private sealed class FailingUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("BD no disponible");
    }
}
