namespace Iperia.Application.Ports.In.Auth;

public interface ILogoutUserUseCase
{
    Task ExecuteAsync(LogoutUserCommand command, CancellationToken cancellationToken = default);
}

/// <param name="TokenId">Identificador único del token (claim jti).</param>
/// <param name="ExpiresAt">Instante en que el token expira por sí solo (UTC).</param>
public sealed record LogoutUserCommand(string TokenId, DateTime ExpiresAt);
