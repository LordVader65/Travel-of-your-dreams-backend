using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.CatalogoRelaciones;

public sealed class ImagenAtraccionDataService : IImagenAtraccionDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public ImagenAtraccionDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<ImagenAtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.ImagenAtracciones.ListarAsync(cancellationToken)).Select(ImagenAtraccionDataMapper.ToDataModel).ToList();
    public async Task<ImagenAtraccionDataModel> CrearAsync(ImagenAtraccionDataModel model, CancellationToken cancellationToken = default) { var e = ImagenAtraccionDataMapper.ToEntity(model); await _unitOfWork.ImagenAtracciones.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return ImagenAtraccionDataMapper.ToDataModel(e); }
    public async Task<ImagenAtraccionDataModel> GuardarAsync(ImagenAtraccionDataModel model, CancellationToken cancellationToken = default)
    {
        var relaciones = await _unitOfWork.ImagenAtracciones.ListarAsync(cancellationToken);
        if (model.EsPrincipal)
        {
            foreach (var item in relaciones.Where(x => x.at_id == model.AtraccionId))
            {
                item.ima_es_principal = false;
            }
        }

        var entity = relaciones.FirstOrDefault(x => x.at_id == model.AtraccionId && x.img_id == model.ImagenId);
        if (entity is null)
        {
            entity = ImagenAtraccionDataMapper.ToEntity(model);
            await _unitOfWork.ImagenAtracciones.AgregarAsync(entity, cancellationToken);
        }
        else
        {
            entity.ima_estado = model.Estado;
            entity.ima_es_principal = model.EsPrincipal;
            entity.ima_orden = model.Orden;
            entity.ima_usuario_ingreso = model.UsuarioIngreso;
            entity.ima_fecha_eliminacion = null;
            entity.ima_usuario_eliminacion = null;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ImagenAtraccionDataMapper.ToDataModel(entity);
    }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.ImagenAtracciones.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.ImagenAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
    public async Task RemoverAsync(int atraccionId, int imagenId, CancellationToken cancellationToken = default) { var e = await _unitOfWork.ImagenAtracciones.ObtenerPorRelacionAsync(atraccionId, imagenId, cancellationToken); if (e is null) return; _unitOfWork.ImagenAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
