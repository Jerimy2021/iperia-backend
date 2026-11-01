using System.IdentityModel.Tokens.Jwt;
using Iperia.Domain.Users;
using Iperia.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace Iperia.Tests.Infrastructure;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void Generate_IncluyeClaimsDelUsuarioYExpiracion()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "iperia-api",
            Audience = "iperia-clients",
            SecretKey = new string('k', 32),
            ExpirationMinutes = 30
        });
        var user = User.Register("Ana Pérez", Email.Create("ana@mail.com"), "hash", Role.Supervisor);

        var token = new JwtTokenGenerator(options).Generate(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);
        Assert.Equal("iperia-api", jwt.Issuer);
        Assert.Equal(user.Id.ToString(), jwt.Subject);
        Assert.Equal("ana@mail.com", jwt.Claims.First(c => c.Type == JwtClaims.Email).Value);
        Assert.Equal("Supervisor", jwt.Claims.First(c => c.Type == JwtClaims.Role).Value);
        Assert.InRange(token.ExpiresAt, DateTime.UtcNow.AddMinutes(29), DateTime.UtcNow.AddMinutes(31));
    }
}
