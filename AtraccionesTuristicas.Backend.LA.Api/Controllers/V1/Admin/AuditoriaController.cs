using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auditoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/auditoria")]
[Authorize(Policy = "AdminOnly")]
public sealed class AuditoriaController : ApiControllerBase
{
    private readonly IAuditoriaLogService _auditoria;

    public AuditoriaController(IAuditoriaLogService auditoria, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _auditoria = auditoria;

    [HttpGet]
    public async Task<IActionResult> Consultar([FromQuery] string tabla, CancellationToken ct) =>
        OkEnvelope(await _auditoria.ConsultarPorTablaAsync(tabla, ct));
}
