using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/usuarios")]
[Authorize(Policy = "AdminOnly")]
public sealed class UsuariosController : ApiControllerBase
{
    private readonly IUsuarioService _usuarios;
    private readonly IRolService _roles;

    public UsuariosController(IUsuarioService usuarios, IRolService roles, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _usuarios = usuarios;
        _roles = roles;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken) =>
        OkEnvelope(await _usuarios.ListarAsync(cancellationToken));

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken cancellationToken) =>
        OkEnvelope(await _usuarios.ObtenerPorGuidAsync(guid, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Crear(CrearUsuarioRequest request, CancellationToken cancellationToken)
    {
        request.UsuarioRegistro = CurrentUser.Login;
        request.IpRegistro = CurrentUser.Ip;
        return CreatedEnvelope(await _usuarios.CrearAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoUsuarioRequest request, CancellationToken cancellationToken)
    {
        request.Guid = guid;
        request.Usuario = CurrentUser.Login;
        request.Ip = CurrentUser.Ip;
        return OkEnvelope(await _usuarios.CambiarEstadoAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPut("{usuarioId:int}/roles")]
    public async Task<IActionResult> CambiarRoles(int usuarioId, CambiarRolUsuarioRequest request, CancellationToken cancellationToken)
    {
        request.UsuarioId = usuarioId;
        return OkEnvelope(await _usuarios.CambiarRolesAsync(request, CurrentUser, cancellationToken));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> Roles(CancellationToken cancellationToken) =>
        OkEnvelope(await _roles.ListarAsync(cancellationToken));
}
