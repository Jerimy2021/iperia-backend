using Iperia.Api.Contracts.Auth;
using Iperia.Application.Ports.In.Auth;
using Iperia.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iperia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRegisterUserUseCase _registerUser;
    private readonly ILoginUserUseCase _loginUser;

    public AuthController(IRegisterUserUseCase registerUser, ILoginUserUseCase loginUser)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await _registerUser.ExecuteAsync(
            new RegisterUserCommand(request.FullName, request.Email, request.Password),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginUser.ExecuteAsync(
            new LoginUserCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Devuelve los datos del usuario autenticado según su token.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me() => Ok(new
    {
        Id = User.FindFirst(JwtClaims.Subject)?.Value,
        Email = User.FindFirst(JwtClaims.Email)?.Value,
        FullName = User.Identity?.Name,
        Role = User.FindFirst(JwtClaims.Role)?.Value
    });
}
