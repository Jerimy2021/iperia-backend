using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Users;
using Iperia.Application.UseCases.Users;
using Iperia.Domain.Users;
using Iperia.Tests.Fakes;

namespace Iperia.Tests.Application.Users;

public class ChangeUserRoleUseCaseTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly ChangeUserRoleUseCase _useCase;
    private readonly User _admin;
    private readonly User _worker;

    public ChangeUserRoleUseCaseTests()
    {
        _admin = User.Register("Admin", Email.Create("admin@mail.com"), "hash", Role.Admin);
        _worker = User.Register("Luis", Email.Create("luis@mail.com"), "hash");
        _users.Users.AddRange(new[] { _admin, _worker });
        _useCase = new ChangeUserRoleUseCase(_users, _unitOfWork);
    }

    [Theory]
    [InlineData("Supervisor", Role.Supervisor)]
    [InlineData("admin", Role.Admin)]
    public async Task CambiaElRol_YGuarda(string role, Role expected)
    {
        var result = await _useCase.ExecuteAsync(new ChangeUserRoleCommand(_worker.Id, role, _admin.Id));

        Assert.Equal(expected, _worker.Role);
        Assert.Equal(expected.ToString(), result.Role);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Theory]
    [InlineData("Gerente")]
    [InlineData("5")]
    [InlineData("0")]
    [InlineData("")]
    public async Task RolInvalido_LanzaValidation(string role)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(new ChangeUserRoleCommand(_worker.Id, role, _admin.Id)));
        Assert.Equal(Role.Worker, _worker.Role);
    }

    [Fact]
    public async Task CambiarSuPropioRol_LanzaValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(new ChangeUserRoleCommand(_admin.Id, "Worker", _admin.Id)));
        Assert.Equal(Role.Admin, _admin.Role);
    }

    [Fact]
    public async Task UsuarioInexistente_LanzaNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _useCase.ExecuteAsync(new ChangeUserRoleCommand(Guid.NewGuid(), "Supervisor", _admin.Id)));
        Assert.Equal(0, _unitOfWork.SaveCount);
    }
}
