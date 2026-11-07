using Iperia.Domain.Evidences;
using Iperia.Domain.Exceptions;

namespace Iperia.Tests.Domain;

public class GeoLocationTests
{
    [Theory]
    [InlineData(-12.0464, -77.0428)]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(0, 0)]
    public void CoordenadasValidas_SeCrean(double lat, double lon)
    {
        var location = GeoLocation.Create(lat, lon);

        Assert.Equal(lat, location.Latitude);
        Assert.Equal(lon, location.Longitude);
    }

    [Theory]
    [InlineData(90.0001, 0)]
    [InlineData(-90.0001, 0)]
    [InlineData(0, 180.0001)]
    [InlineData(0, -180.0001)]
    [InlineData(double.NaN, 0)]
    [InlineData(0, double.NaN)]
    [InlineData(double.PositiveInfinity, 0)]
    public void CoordenadasFueraDeRango_LanzaDomainException(double lat, double lon)
    {
        Assert.Throws<DomainException>(() => GeoLocation.Create(lat, lon));
    }

    [Fact]
    public void MismasCoordenadas_SonIguales()
    {
        Assert.Equal(GeoLocation.Create(-12.5, -77.5), GeoLocation.Create(-12.5, -77.5));
    }
}
