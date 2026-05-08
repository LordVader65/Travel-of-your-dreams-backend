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
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.ImagenAtracciones.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.ImagenAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
