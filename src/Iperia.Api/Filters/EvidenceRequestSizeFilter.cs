using Iperia.Application.UseCases.Evidences;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Iperia.Api.Filters;

/// <summary>
/// Corta las subidas demasiado grandes antes de leer el cuerpo. [RequestSizeLimit] no sirve aquí
/// porque su valor es fijo y el límite de evidencias se configura en appsettings.
/// </summary>
public sealed class EvidenceRequestSizeFilter : IResourceFilter
{
    // Margen para los demás campos del formulario (latitud, longitud, cabeceras multipart).
    private const long FormOverheadBytes = 1024 * 1024;

    private readonly EvidenceUploadOptions _options;

    public EvidenceRequestSizeFilter(EvidenceUploadOptions options)
    {
        _options = options;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var feature = context.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (feature is { IsReadOnly: false })
        {
            feature.MaxRequestBodySize = _options.MaxSizeBytes + FormOverheadBytes;
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
