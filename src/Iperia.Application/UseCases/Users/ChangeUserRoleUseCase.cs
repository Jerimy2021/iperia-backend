using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.In.Users;
using Iperia.Application.Ports.Out;
using Iperia.Domain.Users;

namespace Iperia.Application.UseCases.Users;

public sealed class ChangeUserRoleUseCase : IChangeUserRoleUseCase
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeUserRoleUseCase(IUserRepository users, IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResult> ExecuteAsync(ChangeUserRoleCommand command, CancellationToken cancellationToken = default)
    {
        // Enum.TryParse también acepta números ("0", "5"): solo se permiten los nombres de los roles.
        if (!Enum.TryParse<Role>(command.Role, ignoreCase: true, out var role)
            || !Enum.IsDefined(role)
            || int.TryParse(command.Role, out _))
        {
            var validRoles = string.Join(", ", Enum.GetNames<Role>());
            throw new ValidationException($"Rol inválido. Valores permitidos: {validRoles}.");
        }

        // Evita que el último Admin se quite el rol a sí mismo y el sistema quede sin administradores.
        if (command.UserId == command.RequestedBy)
        {
            throw new ValidationException("No puedes cambiar tu propio rol.");
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("Usuario no encontrado.");

        user.ChangeRole(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserResult.From(user);
    }
}
