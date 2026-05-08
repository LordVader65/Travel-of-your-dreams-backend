namespace AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;

public sealed class ImagenAtraccionService : IImagenAtraccionService
    {
        private readonly IImagenAtraccionDataService _data; public ImagenAtraccionService(IImagenAtraccionDataService data) => _data = data;
        public async Task<IReadOnlyList<ImagenAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(x => new ImagenAtraccionResponse { ImagenId = x.ImagenId, AtraccionId = x.AtraccionId, EsPrincipal = x.EsPrincipal, Orden = x.Orden, Estado = x.Estado }).ToList();
        public async Task<ImagenAtraccionResponse> AsociarAsync(AsociarImagenAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var x = await _data.CrearAsync(new ImagenAtraccionDataModel { ImagenId = request.ImagenId, AtraccionId = request.AtraccionId, EsPrincipal = request.EsPrincipal, Orden = request.Orden, UsuarioIngreso = request.UsuarioIngreso, Estado = BusinessConstants.EstadoActivo }, cancellationToken); return new ImagenAtraccionResponse { ImagenId = x.ImagenId, AtraccionId = x.AtraccionId, EsPrincipal = x.EsPrincipal, Orden = x.Orden, Estado = x.Estado }; }
        public async Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
    }

