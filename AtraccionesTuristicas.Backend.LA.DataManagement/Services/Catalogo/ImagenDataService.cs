using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;

public sealed class ImagenDataService : IImagenDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public ImagenDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<ImagenDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Imagenes.ListarAsync(cancellationToken)).Select(ImagenDataMapper.ToDataModel).ToList();
    public async Task<ImagenDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.Imagenes.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? ImagenDataMapper.ToDataModel(e) : null;
    public async Task<ImagenDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _unitOfWork.Imagenes.ObtenerPorGuidAsync(guid, cancellationToken)) is { } e ? ImagenDataMapper.ToDataModel(e) : null;
    public async Task<ImagenDataModel> CrearAsync(ImagenDataModel model, CancellationToken cancellationToken = default) { var e = ImagenDataMapper.ToEntity(model); await _unitOfWork.Imagenes.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return ImagenDataMapper.ToDataModel(e); }
    public async Task<ImagenDataModel> ActualizarAsync(ImagenDataModel model, CancellationToken cancellationToken = default) { var e = ImagenDataMapper.ToEntity(model); _unitOfWork.Imagenes.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return ImagenDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Imagenes.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Imagenes.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
