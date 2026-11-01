using System.ComponentModel.DataAnnotations;

namespace Iperia.Api.Contracts.Auth;

public sealed record RegisterRequest(
    [Required] string FullName,
    [Required] string Email,
    [Required] string Password);
