using AtraccionesTuristicas.Backend.LA.Api.Models.Requests;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/resenias")]
[Authorize(Policy = "AdminOnly")]
public sealed class ReseniasAdminController : ApiControllerBase
{
    private readonly IReseniaService _resenias;

    public ReseniasAdminController(IReseniaService resenias, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _resenias = resenias;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        OkEnvelope(await _resenias.ListarAdminAsync(CurrentUser, ct));

    [HttpPut("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoApiRequest request, CancellationToken ct) =>
        OkEnvelope(await _resenias.CambiarEstadoAsync(id, request.Estado, CurrentUser, ct));
}
