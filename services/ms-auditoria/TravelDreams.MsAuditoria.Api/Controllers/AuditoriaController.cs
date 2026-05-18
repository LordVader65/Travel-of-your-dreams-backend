using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAuditoria.Business.DTOs;
using TravelDreams.MsAuditoria.Business.Interfaces;

namespace TravelDreams.MsAuditoria.Api.Controllers;

[ApiController]
[Route("api/v1/admin/auditoria")]
public sealed class AuditoriaController : ControllerBase
{
    private readonly IAuditoriaLogService _auditoria;

    public AuditoriaController(IAuditoriaLogService auditoria) => _auditoria = auditoria;

    [HttpGet]
    public async Task<IActionResult> Consultar([FromQuery] AuditoriaLogFiltroRequest filtro, CancellationToken ct)
    {
        var data = await _auditoria.ConsultarAsync(filtro, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _auditoria.ObtenerAsync(guid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Registro de auditoria no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("tablas/{tabla}")]
    public async Task<IActionResult> ConsultarPorTabla(string tabla, CancellationToken ct)
    {
        var data = await _auditoria.ConsultarPorTablaAsync(tabla, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }
}
