using TravelDreams.MsAtracciones.DataManagement.Models.Admin;

namespace TravelDreams.MsAtracciones.DataManagement.Interfaces;

public interface IAdminAtraccionesDataService
{
    Task<IReadOnlyList<CatalogoItemDataModel>> ListarDestinosAsync(CancellationToken ct = default);
    Task<CatalogoItemDataModel> GuardarDestinoAsync(CatalogoUpsertDataModel model, CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoItemDataModel>> ListarCategoriasAsync(CancellationToken ct = default);
    Task<CatalogoItemDataModel> GuardarCategoriaAsync(CatalogoUpsertDataModel model, CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoItemDataModel>> ListarIdiomasAsync(CancellationToken ct = default);
    Task<CatalogoItemDataModel> GuardarIdiomaAsync(CatalogoUpsertDataModel model, CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoItemDataModel>> ListarImagenesAsync(CancellationToken ct = default);
    Task<CatalogoItemDataModel> GuardarImagenAsync(CatalogoUpsertDataModel model, CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoItemDataModel>> ListarIncluyeAsync(CancellationToken ct = default);
    Task<CatalogoItemDataModel> GuardarIncluyeAsync(CatalogoUpsertDataModel model, CancellationToken ct = default);
    Task<bool> DesactivarCatalogoAsync(string tipo, int id, string usuario, CancellationToken ct = default);

    Task<IReadOnlyList<AtraccionAdminDataModel>> ListarAtraccionesAsync(CancellationToken ct = default);
    Task<AtraccionAdminDataModel?> ObtenerAtraccionAsync(Guid guid, CancellationToken ct = default);
    Task<AtraccionAdminDataModel> GuardarAtraccionAsync(AtraccionUpsertDataModel model, CancellationToken ct = default);
    Task<bool> DesactivarAtraccionAsync(Guid guid, string usuario, CancellationToken ct = default);

    Task<IReadOnlyList<TicketAdminDataModel>> ListarTicketsAsync(CancellationToken ct = default);
    Task<TicketAdminDataModel> GuardarTicketAsync(TicketUpsertDataModel model, CancellationToken ct = default);
    Task<bool> DesactivarTicketAsync(Guid guid, string usuario, CancellationToken ct = default);

    Task<IReadOnlyList<HorarioAdminDataModel>> ListarHorariosAsync(CancellationToken ct = default);
    Task<HorarioAdminDataModel> GuardarHorarioAsync(HorarioUpsertDataModel model, CancellationToken ct = default);
    Task<bool> DesactivarHorarioAsync(Guid guid, string usuario, CancellationToken ct = default);

    Task<IReadOnlyList<ReseniaDataModel>> ListarReseniasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ReseniaDataModel>> ListarReseniasPorAtraccionAsync(Guid atraccionGuid, CancellationToken ct = default);
    Task<ReseniaDataModel> CrearReseniaAsync(CrearReseniaDataModel model, CancellationToken ct = default);
    Task<bool> CambiarEstadoReseniaAsync(Guid guid, string estado, string usuario, CancellationToken ct = default);
}
