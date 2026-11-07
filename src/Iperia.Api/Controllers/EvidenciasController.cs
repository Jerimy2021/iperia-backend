using Iperia.Api.Contracts.Evidences;
using Iperia.Api.Filters;
using Iperia.Application.Ports.In.Evidences;
using Iperia.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iperia.Api.Controllers;

[ApiController]
[Route("api/evidencias")]
[Authorize]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
public class EvidenciasController : ControllerBase
{
    private readonly IUploadEvidenceUseCase _uploadEvidence;

    public EvidenciasController(IUploadEvidenceUseCase uploadEvidence)
    {
        _uploadEvidence = uploadEvidence;
    }

    /// <summary>
    /// Sube una imagen como evidencia y guarda su ubicación GPS, la fecha-hora del servidor y al responsable.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ServiceFilter(typeof(EvidenceRequestSizeFilter))]
    [ProducesResponseType(typeof(EvidenceResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] UploadEvidenceRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirst(JwtClaims.Subject)!.Value);

        await using var content = request.File.OpenReadStream();

        var evidence = await _uploadEvidence.ExecuteAsync(
            new UploadEvidenceCommand(content, request.File.Length, request.Latitude!.Value, request.Longitude!.Value, userId),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, evidence);
    }
}
