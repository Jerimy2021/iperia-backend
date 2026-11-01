using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.Out;
using Iperia.Domain.Users;

namespace Iperia.Application.UseCases.Auth;

public sealed class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserUseCase(IUserRepository users, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResult> ExecuteAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(command.Email);
        PasswordPolicy.EnsureIsValid(command.Password);

        if (await _users.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new ConflictException("Ya existe un usuario registrado con ese email.");
        }

        var user = User.Register(command.FullName, email, _passwordHasher.Hash(command.Password));

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserResult.From(user);
    }
}
