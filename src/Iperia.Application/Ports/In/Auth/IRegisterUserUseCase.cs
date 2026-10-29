namespace Iperia.Application.Ports.In.Auth;

public interface IRegisterUserUseCase
{
    Task<UserResult> ExecuteAsync(RegisterUserCommand command, CancellationToken cancellationToken = default);
}

public sealed record RegisterUserCommand(string FullName, string Email, string Password);
