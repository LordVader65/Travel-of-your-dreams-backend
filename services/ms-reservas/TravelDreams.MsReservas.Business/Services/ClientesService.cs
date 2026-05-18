using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;
using TravelDreams.MsReservas.DataManagement.Interfaces;
using TravelDreams.MsReservas.DataManagement.Models;

namespace TravelDreams.MsReservas.Business.Services;

public sealed class ClientesService : IClientesService
{
    private readonly IClientesDataService _data;

    public ClientesService(IClientesDataService data) => _data = data;

    public async Task<IReadOnlyList<ClienteResponse>> ListarAsync(string? numeroIdentificacion, string? correo, string? estado, CancellationToken ct = default) =>
        (await _data.ListarAsync(numeroIdentificacion, correo, estado, ct)).Select(Map).ToList();

    public async Task<ClienteResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var cliente = await _data.ObtenerPorGuidAsync(guid, ct);
        return cliente is null ? null : Map(cliente);
    }

    public async Task<ClienteResponse?> ObtenerPorUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default)
    {
        var cliente = await _data.ObtenerPorUsuarioGuidAsync(usuarioGuid, ct);
        return cliente is null ? null : Map(cliente);
    }

    public async Task<ClienteResponse?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken ct = default)
    {
        var cliente = await _data.ObtenerPorIdentificacionAsync(numeroIdentificacion, ct);
        return cliente is null ? null : Map(cliente);
    }

    public async Task<ClienteResponse> GuardarAsync(ClienteRequest request, CancellationToken ct = default)
    {
        Validate(request);
        var model = new ClienteDataModel
        {
            Guid = request.ClienteGuid,
            UsuarioGuid = request.UsuarioGuid,
            TipoIdentificacion = request.TipoIdentificacion.Trim().ToUpperInvariant(),
            NumeroIdentificacion = request.NumeroIdentificacion.Trim(),
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            RazonSocial = request.RazonSocial,
            Correo = request.Correo.Trim(),
            Telefono = request.Telefono,
            Direccion = request.Direccion
        };
        return Map(await _data.GuardarAsync(model, ct));
    }

    public Task<bool> CambiarEstadoAsync(Guid guid, CambiarEstadoClienteRequest request, CancellationToken ct = default)
    {
        if (request.Estado is not ("A" or "I")) throw new InvalidOperationException("Estado de cliente no permitido.");
        return _data.CambiarEstadoAsync(guid, request.Estado, "admin", "api", ct);
    }

    private static void Validate(ClienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TipoIdentificacion)) throw new InvalidOperationException("TipoIdentificacion es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.NumeroIdentificacion)) throw new InvalidOperationException("NumeroIdentificacion es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Correo) || !request.Correo.Contains('@')) throw new InvalidOperationException("Correo invalido.");
        if (request.Telefono?.Length > 20) throw new InvalidOperationException("Telefono demasiado largo.");
    }

    private static ClienteResponse Map(ClienteDataModel model) => new()
    {
        Guid = model.Guid ?? Guid.Empty,
        UsuarioGuid = model.UsuarioGuid,
        TipoIdentificacion = model.TipoIdentificacion,
        NumeroIdentificacion = model.NumeroIdentificacion,
        Nombres = model.Nombres,
        Apellidos = model.Apellidos,
        RazonSocial = model.RazonSocial,
            Correo = model.Correo,
            Telefono = model.Telefono,
            Direccion = model.Direccion,
            Estado = model.Estado
        };
}
