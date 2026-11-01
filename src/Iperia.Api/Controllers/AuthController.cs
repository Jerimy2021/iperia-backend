using Iperia.Api.Contracts.Auth;
using Iperia.Application.Ports.In.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Iperia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRegisterUserUseCase _registerUser;

    public AuthController(IRegisterUserUseCase registerUser)
    {
        _registerUser = registerUser;
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
}
