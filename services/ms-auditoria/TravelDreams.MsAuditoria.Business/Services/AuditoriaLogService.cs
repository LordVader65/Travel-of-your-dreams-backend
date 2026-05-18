using TravelDreams.MsAuditoria.Business.DTOs;
using TravelDreams.MsAuditoria.Business.Interfaces;
using TravelDreams.MsAuditoria.DataManagement.Interfaces;
using TravelDreams.MsAuditoria.DataManagement.Models;

namespace TravelDreams.MsAuditoria.Business.Services;

public sealed class AuditoriaLogService : IAuditoriaLogService
{
    private static readonly string[] ValidOperations = ["INSERT", "UPDATE", "DELETE", "LOGIN", "LOGOUT", "BUSINESS_EVENT"];
    private readonly IAuditoriaLogDataService _data;

    public AuditoriaLogService(IAuditoriaLogDataService data) => _data = data;

    public async Task<PagedResponse<AuditoriaLogResponse>> ConsultarAsync(AuditoriaLogFiltroRequest filtro, CancellationToken ct = default)
    {
        var result = await _data.ConsultarAsync(new AuditoriaFiltroDataModel
        {
            Servicio = filtro.Servicio,
            Tabla = filtro.Tabla,
            Operacion = filtro.Operacion,
            Usuario = filtro.Usuario,
            CorrelationId = filtro.CorrelationId,
            DesdeUtc = filtro.DesdeUtc,
            HastaUtc = filtro.HastaUtc,
            Page = filtro.Page,
            Limit = filtro.Limit
        }, ct);

        return new PagedResponse<AuditoriaLogResponse>
        {
            Items = result.Items.Select(Map).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public async Task<IReadOnlyList<AuditoriaLogResponse>> ConsultarPorTablaAsync(string tabla, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(tabla)) throw new InvalidOperationException("Tabla es obligatoria.");
        return (await _data.ConsultarPorTablaAsync(tabla, ct)).Select(Map).ToList();
    }

    public async Task<AuditoriaLogResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var model = await _data.ObtenerAsync(guid, ct);
        return model is null ? null : Map(model);
    }

    public async Task<long> RegistrarAsync(RegistrarAuditoriaRequest request, CancellationToken ct = default)
    {
        Validate(request);
        return await _data.RegistrarAsync(new AuditoriaLogDataModel
        {
            EventoId = request.EventoId,
            Servicio = request.Servicio.Trim(),
            Tabla = request.Tabla.Trim(),
            Operacion = request.Operacion.Trim().ToUpperInvariant(),
            RegistroId = request.RegistroId,
            RegistroGuid = request.RegistroGuid,
            DatosAnteriores = request.DatosAnteriores,
            DatosNuevos = request.DatosNuevos,
            FechaUtc = request.FechaUtc ?? DateTime.UtcNow,
            Usuario = request.Usuario.Trim(),
            Ip = request.Ip.Trim(),
            OrigenCanal = request.OrigenCanal,
            CorrelationId = request.CorrelationId
        }, ct);
    }

    private static void Validate(RegistrarAuditoriaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Servicio)) throw new InvalidOperationException("Servicio es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Tabla)) throw new InvalidOperationException("Tabla es obligatoria.");
        if (string.IsNullOrWhiteSpace(request.Operacion)) throw new InvalidOperationException("Operacion es obligatoria.");
        if (!ValidOperations.Contains(request.Operacion.Trim().ToUpperInvariant())) throw new InvalidOperationException("Operacion no permitida.");
        if (string.IsNullOrWhiteSpace(request.Usuario)) throw new InvalidOperationException("Usuario es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Ip)) throw new InvalidOperationException("Ip es obligatoria.");
        if (request.Operacion.Trim().Equals("INSERT", StringComparison.OrdinalIgnoreCase) && request.DatosAnteriores is not null) throw new InvalidOperationException("INSERT no debe tener datos anteriores.");
        if (request.Operacion.Trim().Equals("DELETE", StringComparison.OrdinalIgnoreCase) && request.DatosNuevos is not null) throw new InvalidOperationException("DELETE no debe tener datos nuevos.");
    }

    private static AuditoriaLogResponse Map(AuditoriaLogDataModel model) => new()
    {
        Id = model.Id,
        Guid = model.Guid,
        Servicio = model.Servicio,
        Tabla = model.Tabla,
        Operacion = model.Operacion,
        RegistroId = model.RegistroId,
        RegistroGuid = model.RegistroGuid,
        DatosAnteriores = model.DatosAnteriores,
        DatosNuevos = model.DatosNuevos,
        FechaUtc = model.FechaUtc,
        Usuario = model.Usuario,
        Ip = model.Ip,
        OrigenCanal = model.OrigenCanal,
        CorrelationId = model.CorrelationId,
        EventoId = model.EventoId
    };
}
