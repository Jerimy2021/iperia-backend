using Iperia.Application.Ports.Out;

namespace Iperia.Tests.Fakes;

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string passwordHash) => passwordHash == Hash(password);
}
