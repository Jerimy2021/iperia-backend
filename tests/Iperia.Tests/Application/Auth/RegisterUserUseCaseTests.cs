using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.UseCases.Auth;
using Iperia.Domain.Exceptions;
using Iperia.Tests.Fakes;

namespace Iperia.Tests.Application.Auth;

public class RegisterUserUseCaseTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly RegisterUserUseCase _useCase;

    public RegisterUserUseCaseTests()
    {
        _useCase = new RegisterUserUseCase(_users, new FakePasswordHasher(), _unitOfWork);
    }

    [Fact]
    public async Task Registra_UsuarioConPasswordHasheadoYRolWorker()
    {
        var result = await _useCase.ExecuteAsync(new RegisterUserCommand("Ana Pérez", "Ana@Mail.com", "Secreto123"));

        var saved = Assert.Single(_users.Users);
        Assert.Equal("ana@mail.com", result.Email);
        Assert.Equal("Worker", result.Role);
        Assert.Equal("hashed:Secreto123", saved.PasswordHash);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task EmailDuplicado_LanzaConflict()
    {
        await _useCase.ExecuteAsync(new RegisterUserCommand("Ana", "ana@mail.com", "Secreto123"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(new RegisterUserCommand("Otra", "ANA@mail.com", "Secreto123")));
    }

    [Theory]
    [InlineData("corta1A")]
    [InlineData("sinnumeros")]
    [InlineData("SINMINUSCULAS1")]
    [InlineData("sinmayusculas1")]
    public async Task PasswordDebil_LanzaValidation(string password)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(new RegisterUserCommand("Ana", "ana@mail.com", password)));
        Assert.Empty(_users.Users);
    }

    [Fact]
    public async Task NombreVacio_LanzaDomainException()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _useCase.ExecuteAsync(new RegisterUserCommand("   ", "ana@mail.com", "Secreto123")));
    }
}
