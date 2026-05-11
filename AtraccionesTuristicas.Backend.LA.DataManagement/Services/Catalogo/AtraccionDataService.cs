using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;

public sealed class AtraccionDataService : IAtraccionDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public AtraccionDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<AtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default) =>
        (await _unitOfWork.Atracciones.ListarAsync(cancellationToken)).Select(AtraccionDataMapper.ToDataModel).ToList();

    public async Task<AtraccionDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Atracciones.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : AtraccionDataMapper.ToDataModel(entity);
    }

    public async Task<AtraccionDataModel> CrearAsync(AtraccionDataModel model, CancellationToken cancellationToken = default)
    {
        var entity = AtraccionDataMapper.ToEntity(model);
        await _unitOfWork.Atracciones.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return AtraccionDataMapper.ToDataModel(entity);
    }

    public async Task<AtraccionDataModel?> ActualizarAsync(AtraccionDataModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Atracciones.ObtenerParaActualizarAsync(model.Guid, cancellationToken);
        if (entity is null) return null;
        entity.des_id = model.DestinoId; entity.at_nombre = model.Nombre; entity.at_descripcion = model.Descripcion;
        entity.at_direccion = model.Direccion; entity.at_disponible = model.Disponible; entity.at_precio_referencia = model.PrecioReferencia;
        entity.at_duracion_minutos = model.DuracionMinutos; entity.at_punto_encuentro = model.PuntoEncuentro;
        entity.at_free_cancellation = model.FreeCancellation; entity.at_skip_the_line = model.SkipTheLine;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return AtraccionDataMapper.ToDataModel(entity);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Atracciones.ObtenerPorIdAsync(id, cancellationToken);
        if (entity is null) return;
        _unitOfWork.Atracciones.Remover(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DataPagedResult<AtraccionPublicaDataModel>> ListarPublicasAsync(AtraccionFiltroDataModel filtro, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.AtraccionQueries.ListarPublicasAsync(
            filtro.Page,
            filtro.Limit,
            filtro.Pais,
            filtro.FechaDesde,
            filtro.FechaHasta,
            filtro.Tipo,
            filtro.Subtipo,
            filtro.Etiqueta,
            filtro.Idioma,
            filtro.PrecioMinimo,
            filtro.PrecioMaximo,
            filtro.RatingMinimo,
            filtro.Horario,
            filtro.OrdenarPor,
            filtro.SoloDisponibles,
            cancellationToken);

        return new DataPagedResult<AtraccionPublicaDataModel>
        {
            Items = result.Items.Select(AtraccionDataMapper.ToPublicDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public async Task<AtraccionPublicaDataModel?> ObtenerDetallePublicoAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.AtraccionQueries.ObtenerDetallePublicoAsync(guid, cancellationToken);
        return entity is null ? null : AtraccionDataMapper.ToPublicDataModel(entity);
    }
}
