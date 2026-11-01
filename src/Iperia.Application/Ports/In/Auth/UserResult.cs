using Iperia.Domain.Users;

namespace Iperia.Application.Ports.In.Auth;

public sealed record UserResult(Guid Id, string FullName, string Email, string Role, DateTime CreatedAt)
{
    public static UserResult From(User user) =>
        new(user.Id, user.FullName, user.Email.Value, user.Role.ToString(), user.CreatedAt);
}
