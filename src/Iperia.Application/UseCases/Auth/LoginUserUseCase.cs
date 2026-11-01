using Iperia.Application.Common.Exceptions;
using Iperia.Application.Ports.In.Auth;
using Iperia.Application.Ports.Out;
using Iperia.Domain.Users;

namespace Iperia.Application.UseCases.Auth;

public sealed class LoginUserUseCase : ILoginUserUseCase
{
    // Mismo mensaje para email inexistente y password incorrecto: no revela qué cuentas existen.
    private const string InvalidCredentials = "Email o password incorrectos.";

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUserUseCase(IUserRepository users, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResult> ExecuteAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        if (!Email.IsValid(command.Email) || string.IsNullOrEmpty(command.Password))
        {
            throw new UnauthorizedException(InvalidCredentials);
        }

        var user = await _users.GetByEmailAsync(Email.Create(command.Email), cancellationToken);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedException(InvalidCredentials);
        }

        var token = _tokenGenerator.Generate(user);

        return new LoginResult(token.Token, token.ExpiresAt, UserResult.From(user));
    }
}
