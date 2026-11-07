using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.UseCases.Auth;
using Iperia.Tests.Fakes;

namespace Iperia.Tests.Application.Auth;

public class LogoutUserUseCaseTests
{
    private readonly InMemoryRevokedTokenStore _store = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly LogoutUserUseCase _useCase;

    public LogoutUserUseCaseTests()
    {
        _useCase = new LogoutUserUseCase(_store, _unitOfWork);
    }

    [Fact]
    public async Task TokenValido_QuedaRevocadoYSeGuarda()
    {
        var expiresAt = DateTime.UtcNow.AddHours(1);

        await _useCase.ExecuteAsync(new LogoutUserCommand("jti-1", expiresAt));

        Assert.True(await _store.IsRevokedAsync("jti-1"));
        Assert.Equal(expiresAt, _store.Revoked["jti-1"]);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task TokenYaRevocado_NoHaceNadaYNoFalla()
    {
        await _useCase.ExecuteAsync(new LogoutUserCommand("jti-1", DateTime.UtcNow.AddHours(1)));
        await _useCase.ExecuteAsync(new LogoutUserCommand("jti-1", DateTime.UtcNow.AddHours(1)));

        Assert.Single(_store.Revoked);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task TokenSinId_LanzaUnauthorized(string tokenId)
    {
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _useCase.ExecuteAsync(new LogoutUserCommand(tokenId, DateTime.UtcNow.AddHours(1))));

        Assert.Empty(_store.Revoked);
    }
}
