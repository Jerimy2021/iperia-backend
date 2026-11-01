using Iperia.Domain.Users;

namespace Iperia.Application.Ports.Out;

public interface ITokenGenerator
{
    AccessToken Generate(User user);
}

public sealed record AccessToken(string Token, DateTime ExpiresAt);
