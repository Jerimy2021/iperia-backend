using Iperia.Domain.Common;
using Iperia.Domain.Exceptions;

namespace Iperia.Domain.Evidences;

/// <summary>
/// Coordenadas GPS (grados decimales) con latitud en [-90, 90] y longitud en [-180, 180].
/// </summary>
public sealed class GeoLocation : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }

    private GeoLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GeoLocation Create(double latitude, double longitude)
    {
        if (double.IsNaN(latitude) || latitude < -90 || latitude > 90)
        {
            throw new DomainException("La latitud debe estar entre -90 y 90.");
        }

        if (double.IsNaN(longitude) || longitude < -180 || longitude > 180)
        {
            throw new DomainException("La longitud debe estar entre -180 y 180.");
        }

        return new GeoLocation(latitude, longitude);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
