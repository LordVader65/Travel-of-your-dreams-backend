using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAuditoria.Business.DTOs;
using TravelDreams.MsAuditoria.Business.Interfaces;

namespace TravelDreams.MsAuditoria.Api.Controllers;

[ApiController]
[Route("internal/v1/auditoria")]
public sealed class InternalAuditoriaController : ControllerBase
{
    private readonly IAuditoriaLogService _auditoria;

    public InternalAuditoriaController(IAuditoriaLogService auditoria) => _auditoria = auditoria;

    [HttpPost("logs")]
    public async Task<IActionResult> Registrar(RegistrarAuditoriaRequest request, CancellationToken ct)
    {
        request.CorrelationId ??= Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId)
            ? correlationId.ToString()
            : null;

        var id = await _auditoria.RegistrarAsync(request, ct);
        return id == 0
            ? Ok(new { status = StatusCodes.Status200OK, data = new { procesado = true, duplicado = true } })
            : Created(string.Empty, new { status = StatusCodes.Status201Created, data = new { id } });
    }
}
