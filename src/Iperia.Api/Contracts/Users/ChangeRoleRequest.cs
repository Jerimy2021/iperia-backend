using System.ComponentModel.DataAnnotations;

namespace Iperia.Api.Contracts.Users;

public sealed record ChangeRoleRequest([Required] string Role);
