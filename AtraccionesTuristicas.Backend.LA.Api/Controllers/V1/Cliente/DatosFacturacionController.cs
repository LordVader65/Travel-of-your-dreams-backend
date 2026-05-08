using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.Exceptions;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Cliente;

[Route("api/v{version:apiVersion}/me/datos-facturacion")]
[Authorize]
public sealed class DatosFacturacionController : ApiControllerBase
{
    private readonly IDatosFacturacionService _datos;
    private readonly IClienteService _clientes;

    public DatosFacturacionController(IDatosFacturacionService datos, IClienteService clientes, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _datos = datos;
        _clientes = clientes;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        return OkEnvelope(await _datos.ListarActivosPorClienteAsync(clienteGuid, CurrentUser, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearDatosFacturacionRequest request, CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        var cliente = await _clientes.ObtenerPorGuidAsync(clienteGuid, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
        request.ClienteId = cliente.Id;
        request.UsuarioIngreso = CurrentUser.Login;
        request.IpIngreso = CurrentUser.Ip;
        return CreatedEnvelope(await _datos.CrearAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, ActualizarDatosFacturacionRequest request, CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        var cliente = await _clientes.ObtenerPorGuidAsync(clienteGuid, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
        request.Guid = guid;
        request.ClienteId = cliente.Id;
        return OkEnvelope(await _datos.ActualizarAsync(request, CurrentUser, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        await _datos.EliminarAsync(id, CurrentUser, cancellationToken);
        return NoContent();
    }
}
