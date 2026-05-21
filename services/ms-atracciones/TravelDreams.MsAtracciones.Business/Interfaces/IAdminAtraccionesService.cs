using TravelDreams.MsAtracciones.Business.DTOs;

namespace TravelDreams.MsAtracciones.Business.Interfaces;

public interface IAdminAtraccionesService
{
    Task<object> ListarCatalogoAsync(string tipo, CancellationToken ct = default);
    Task<object> GuardarCatalogoAsync(string tipo, int? id, AdminCatalogoRequest request, CancellationToken ct = default);
    Task<bool> DesactivarCatalogoAsync(string tipo, int id, CancellationToken ct = default);
    Task<object> ListarAtraccionesAsync(CancellationToken ct = default);
    Task<object?> ObtenerAtraccionAsync(Guid guid, CancellationToken ct = default);
    Task<object> GuardarAtraccionAsync(Guid? guid, AdminAtraccionRequest request, CancellationToken ct = default);
    Task<bool> DesactivarAtraccionAsync(Guid guid, CancellationToken ct = default);
    Task<object> ListarTicketsAsync(CancellationToken ct = default);
    Task<object> GuardarTicketAsync(Guid? guid, AdminTicketRequest request, CancellationToken ct = default);
    Task<bool> DesactivarTicketAsync(Guid guid, CancellationToken ct = default);
    Task<object> ListarHorariosAsync(CancellationToken ct = default);
    Task<object> GuardarHorarioAsync(Guid? guid, AdminHorarioRequest request, CancellationToken ct = default);
    Task<bool> DesactivarHorarioAsync(Guid guid, CancellationToken ct = default);
    Task<int> DesactivarHorariosVencidosAsync(CancellationToken ct = default);
    Task<object> ListarReseniasAsync(CancellationToken ct = default);
    Task<object> ListarReseniasPorAtraccionAsync(Guid atraccionGuid, CancellationToken ct = default);
    Task<object> CrearReseniaAsync(CrearReseniaRequest request, CancellationToken ct = default);
    Task<bool> CambiarEstadoReseniaAsync(Guid guid, string estado, CancellationToken ct = default);
}
