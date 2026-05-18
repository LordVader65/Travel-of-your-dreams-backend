using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.Business.Services;

public sealed class DatosFacturacionService : IDatosFacturacionService
{
    private readonly IDatosFacturacionDataService _data;

    public DatosFacturacionService(IDatosFacturacionDataService data) => _data = data;

    public async Task<IReadOnlyList<DatosFacturacionResponse>> ListarPorClienteAsync(Guid clienteGuid, CancellationToken ct = default) =>
        (await _data.ListarActivosPorClienteAsync(clienteGuid, ct)).Select(Map).ToList();

    public async Task<DatosFacturacionResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var model = await _data.ObtenerPorGuidAsync(guid, ct);
        return model is null ? null : Map(model);
    }

    public async Task<DatosFacturacionResponse> GuardarAsync(DatosFacturacionRequest request, CancellationToken ct = default)
    {
        Validate(request);
        var model = new DatosFacturacionDataModel
        {
            Guid = request.Guid ?? Guid.Empty,
            ClienteGuid = request.ClienteGuid,
            TipoIdentificacion = request.TipoIdentificacion.Trim().ToUpperInvariant(),
            NumeroIdentificacion = request.NumeroIdentificacion.Trim(),
            RazonSocial = request.RazonSocial,
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido,
            Correo = request.Correo.Trim(),
            Telefono = request.Telefono,
            Direccion = request.Direccion
        };
        return Map(await _data.GuardarAsync(model, "cliente", "api", ct));
    }

    public Task<bool> InactivarAsync(Guid guid, CancellationToken ct = default) =>
        _data.InactivarAsync(guid, "cliente", "api", ct);

    private static void Validate(DatosFacturacionRequest request)
    {
        if (request.ClienteGuid == Guid.Empty) throw new InvalidOperationException("ClienteGuid es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.TipoIdentificacion)) throw new InvalidOperationException("TipoIdentificacion es obligatorio.");
        if (!new[] { "CC", "RUC", "PASAPORTE", "CEDULA", "OTRO" }.Contains(request.TipoIdentificacion.Trim().ToUpperInvariant())) throw new InvalidOperationException("TipoIdentificacion no permitido.");
        if (string.IsNullOrWhiteSpace(request.NumeroIdentificacion)) throw new InvalidOperationException("NumeroIdentificacion es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Nombre)) throw new InvalidOperationException("Nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Correo) || !request.Correo.Contains('@')) throw new InvalidOperationException("Correo invalido.");
        if (request.Telefono?.Length > 20) throw new InvalidOperationException("Telefono demasiado largo.");
    }

    internal static DatosFacturacionResponse Map(DatosFacturacionDataModel model) => new()
    {
        Guid = model.Guid,
        ClienteGuid = model.ClienteGuid,
        TipoIdentificacion = model.TipoIdentificacion,
        NumeroIdentificacion = model.NumeroIdentificacion,
        RazonSocial = model.RazonSocial,
        Nombre = model.Nombre,
        Apellido = model.Apellido,
        Correo = model.Correo,
        Telefono = model.Telefono,
        Direccion = model.Direccion,
        Estado = model.Estado
    };
}
