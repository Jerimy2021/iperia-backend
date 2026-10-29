using Iperia.Domain.Exceptions;
using Iperia.Domain.Users;

namespace Iperia.Tests.Domain;

public class EmailTests
{
    [Fact]
    public void Create_NormalizaEspaciosYMayusculas()
    {
        var email = Email.Create("  Ana.Perez@Tecsup.EDU.pe ");

        Assert.Equal("ana.perez@tecsup.edu.pe", email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("sin-arroba.com")]
    [InlineData("a@b")]
    [InlineData("con espacio@dominio.com")]
    public void Create_FormatoInvalido_LanzaDomainException(string? value)
    {
        Assert.Throws<DomainException>(() => Email.Create(value));
    }

    [Fact]
    public void Emails_ConMismoValor_SonIguales()
    {
        Assert.Equal(Email.Create("a@b.com"), Email.Create("A@B.COM"));
    }
}
