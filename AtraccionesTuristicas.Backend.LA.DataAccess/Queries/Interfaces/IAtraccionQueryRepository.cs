using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IAtraccionQueryRepository
{
    Task<PagedResult<AtraccionEntity>> ListarPublicasAsync(
        int page,
        int limit,
        string? pais = null,
        DateOnly? fechaDesde = null,
        DateOnly? fechaHasta = null,
        string? tipo = null,
        string? subtipo = null,
        string? etiqueta = null,
        string? idioma = null,
        decimal? precioMinimo = null,
        decimal? precioMaximo = null,
        short? ratingMinimo = null,
        string? horario = null,
        string? ordenarPor = null,
        bool soloDisponibles = true,
        CancellationToken cancellationToken = default);

    Task<AtraccionEntity?> ObtenerDetallePublicoAsync(Guid guid, CancellationToken cancellationToken = default);
}
