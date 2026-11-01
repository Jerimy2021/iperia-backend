namespace Iperia.Application.Ports.In.Auth;

public interface ILoginUserUseCase
{
    Task<LoginResult> ExecuteAsync(LoginUserCommand command, CancellationToken cancellationToken = default);
}

public sealed record LoginUserCommand(string Email, string Password);

public sealed record LoginResult(string AccessToken, DateTime ExpiresAt, UserResult User);
