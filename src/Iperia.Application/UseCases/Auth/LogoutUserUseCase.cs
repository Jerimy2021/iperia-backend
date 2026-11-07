using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.Out;

namespace Iperia.Application.UseCases.Auth;

public sealed class LogoutUserUseCase : ILogoutUserUseCase
{
    private readonly IRevokedTokenStore _revokedTokens;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUserUseCase(IRevokedTokenStore revokedTokens, IUnitOfWork unitOfWork)
    {
        _revokedTokens = revokedTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(LogoutUserCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.TokenId))
        {
            throw new UnauthorizedException("El token no es válido.");
        }

        // Idempotente: si ya estaba revocado no hay nada más que hacer.
        if (await _revokedTokens.IsRevokedAsync(command.TokenId, cancellationToken))
        {
            return;
        }

        await _revokedTokens.RevokeAsync(command.TokenId, command.ExpiresAt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
