using Iperia.Application.Iperc;

namespace Iperia.Application.Ports.Out;

/// <summary>
/// Puerto de salida que entrega la matriz IPERC vigente (hoy un archivo JSON). Se vuelve a leer si el archivo cambia.
/// </summary>
public interface IIpercMatrixProvider
{
    IpercMatrix GetMatrix();
}
