using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Public;

[Route("api/v{version:apiVersion}/atracciones/{atraccionGuid:guid}/horarios")]
[AllowAnonymous]
public sealed class HorariosController : ApiControllerBase
{
    private readonly IHorarioService _horarios;

    public HorariosController(IHorarioService horarios, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _horarios = horarios;

    [HttpGet]
    public async Task<IActionResult> Listar(Guid atraccionGuid, [FromQuery] DateOnly? fecha, CancellationToken cancellationToken)
    {
        var response = await _horarios.ListarDisponiblesPorAtraccionAsync(new HorarioFiltroRequest { AtraccionGuid = atraccionGuid, Fecha = fecha }, cancellationToken);
        return OkEnvelope(response);
    }
}
