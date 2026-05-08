using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;
using AtraccionesTuristicas.Backend.LA.Business.Exceptions;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Cliente;

[Route("api/v{version:apiVersion}/me")]
[Authorize]
public sealed class PerfilController : ApiControllerBase
{
    private readonly IClienteService _clientes;
    private readonly IUsuarioService _usuarios;

    public PerfilController(IClienteService clientes, IUsuarioService usuarios, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _clientes = clientes;
        _usuarios = usuarios;
    }

    [HttpGet]
    public async Task<IActionResult> Obtener(CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        return OkEnvelope(await _clientes.ObtenerPorGuidAsync(clienteGuid, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado."));
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar(ActualizarPerfilClienteRequest request, CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        var actual = await _clientes.ObtenerPorGuidAsync(clienteGuid, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
        var response = await _clientes.ActualizarAsync(new ActualizarClienteRequest
        {
            Guid = clienteGuid,
            TipoIdentificacion = actual.TipoIdentificacion,
            NumeroIdentificacion = actual.NumeroIdentificacion,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            RazonSocial = actual.RazonSocial,
            Correo = request.Correo,
            Telefono = request.Telefono,
            Direccion = request.Direccion,
            Estado = actual.Estado
        }, cancellationToken);
        return OkEnvelope(response);
    }

    [HttpPut("password")]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordRequest request, CancellationToken cancellationToken)
    {
        request.UsuarioGuid = CurrentUser.UsuarioGuid ?? request.UsuarioGuid;
        request.Usuario = CurrentUser.Login;
        request.Ip = CurrentUser.Ip;
        return OkEnvelope(await _usuarios.CambiarPasswordAsync(request, CurrentUser, cancellationToken));
    }
}
