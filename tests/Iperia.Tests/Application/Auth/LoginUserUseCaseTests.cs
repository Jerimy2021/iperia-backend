using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.Out;
using Iperia.Application.UseCases.Auth;
using Iperia.Domain.Users;
using Iperia.Tests.Fakes;

namespace Iperia.Tests.Application.Auth;

public class LoginUserUseCaseTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly LoginUserUseCase _useCase;

    public LoginUserUseCaseTests()
    {
        _users.Users.Add(User.Register("Ana Pérez", Email.Create("ana@mail.com"), _hasher.Hash("Secreto123")));
        _useCase = new LoginUserUseCase(_users, _hasher, new FakeTokenGenerator());
    }

    [Fact]
    public async Task CredencialesCorrectas_DevuelveToken()
    {
        var result = await _useCase.ExecuteAsync(new LoginUserCommand(" ANA@mail.com ", "Secreto123"));

        Assert.Equal("token-ana@mail.com", result.AccessToken);
        Assert.Equal("ana@mail.com", result.User.Email);
    }

    [Theory]
    [InlineData("ana@mail.com", "Incorrecto123")]
    [InlineData("noexiste@mail.com", "Secreto123")]
    [InlineData("email-invalido", "Secreto123")]
    [InlineData("ana@mail.com", "")]
    public async Task CredencialesInvalidas_LanzaUnauthorizedConMensajeGenerico(string email, string password)
    {
        var ex = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _useCase.ExecuteAsync(new LoginUserCommand(email, password)));

        Assert.Equal("Email o password incorrectos.", ex.Message);
    }

    private sealed class FakeTokenGenerator : ITokenGenerator
    {
        public AccessToken Generate(User user) => new($"token-{user.Email}", DateTime.UtcNow.AddHours(1));
    }
}
