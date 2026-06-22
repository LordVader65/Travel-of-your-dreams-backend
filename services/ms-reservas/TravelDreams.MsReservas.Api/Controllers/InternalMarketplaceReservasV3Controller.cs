using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDreams.MsReservas.DataAccess.Context;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("internal/v3/marketplace/reservas/solicitudes")]
public sealed class InternalMarketplaceReservasV3Controller : ControllerBase
{
    private readonly ReservasDbContext _db;

    public InternalMarketplaceReservasV3Controller(ReservasDbContext db) => _db = db;

    [HttpGet("{correlationId:guid}")]
    public async Task<IActionResult> Obtener(Guid correlationId, [FromQuery] Guid clienteGuid, CancellationToken ct)
    {
        if (clienteGuid == Guid.Empty)
            return BadRequest(new { status = 400, error = "clienteGuid es obligatorio." });

        var item = await _db.MarketplaceReservaSolicitudesV3.AsNoTracking()
            .Where(x => x.rsol_correlation_id == correlationId && x.cli_guid == clienteGuid)
            .Select(x => new
            {
                correlationId = x.rsol_correlation_id,
                estado = x.rsol_estado,
                reservaGuid = x.rev_guid,
                reservaCodigo = x.rev_codigo,
                error = x.rsol_error,
                createdAtUtc = x.rsol_created_at_utc,
                updatedAtUtc = x.rsol_updated_at_utc
            })
            .FirstOrDefaultAsync(ct);

        return item is null
            ? NotFound(new { status = 404, error = "Solicitud de reserva no encontrada." })
            : Ok(new { status = 200, data = item });
    }
}
