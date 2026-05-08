using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/horarios")]
[Authorize(Policy = "AdminOnly")]
public sealed class HorariosAdminController : ApiControllerBase
{
    private readonly IHorarioService _horarios;

    public HorariosAdminController(IHorarioService horarios, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _horarios = horarios;

    [HttpGet] public async Task<IActionResult> Listar(CancellationToken ct) => OkEnvelope(await _horarios.ListarAsync(CurrentUser, ct));
    [HttpPost] public async Task<IActionResult> Crear(CrearHorarioRequest request, CancellationToken ct) { request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip; return CreatedEnvelope(await _horarios.CrearAsync(request, CurrentUser, ct)); }
    [HttpPut("{guid:guid}")] public async Task<IActionResult> Actualizar(Guid guid, ActualizarHorarioRequest request, CancellationToken ct) { request.Guid = guid; request.UsuarioModificacion = CurrentUser.Login; request.IpModificacion = CurrentUser.Ip; return OkEnvelope(await _horarios.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpPut("{guid:guid}/estado")] public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoHorarioRequest request, CancellationToken ct) { request.Guid = guid; request.Usuario = CurrentUser.Login; request.Ip = CurrentUser.Ip; return OkEnvelope(await _horarios.CambiarEstadoAsync(request, CurrentUser, ct)); }
    [HttpPost("desactivar-vencidos")] public async Task<IActionResult> DesactivarVencidos(CancellationToken ct) => OkEnvelope(new { total = await _horarios.DesactivarPasadosOSinCuposAsync(CurrentUser, ct) });
}
