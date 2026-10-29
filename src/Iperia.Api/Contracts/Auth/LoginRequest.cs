using System.ComponentModel.DataAnnotations;

namespace Iperia.Api.Contracts.Auth;

public sealed record LoginRequest(
    [Required] string Email,
    [Required] string Password);
