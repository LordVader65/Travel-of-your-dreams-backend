using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.DataAccess.Context;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("internal/v3/marketplace/pagos/solicitudes")]
public sealed class InternalMarketplacePagosV3Controller : ControllerBase
{
    private readonly FacturacionDbContext _db;

    public InternalMarketplacePagosV3Controller(FacturacionDbContext db) => _db = db;

    [HttpGet("{correlationId:guid}")]
    public async Task<IActionResult> Obtener(Guid correlationId, [FromQuery] Guid clienteGuid, CancellationToken ct)
    {
        if (clienteGuid == Guid.Empty)
            return BadRequest(new { status = 400, error = "clienteGuid es obligatorio." });

        var item = await _db.MarketplacePagoSolicitudesV3.AsNoTracking()
            .Where(x => x.fsol_correlation_id == correlationId && x.cli_guid == clienteGuid)
            .Select(x => new
            {
                correlationId = x.fsol_correlation_id,
                estado = x.fsol_estado,
                reservaGuid = x.rev_guid,
                facturaGuid = x.fac_guid,
                facturaNumero = x.fac_numero,
                error = x.fsol_error,
                createdAtUtc = x.fsol_created_at_utc,
                updatedAtUtc = x.fsol_updated_at_utc
            })
            .FirstOrDefaultAsync(ct);

        return item is null
            ? NotFound(new { status = 404, error = "Solicitud de pago no encontrada." })
            : Ok(new { status = 200, data = item });
    }
}
