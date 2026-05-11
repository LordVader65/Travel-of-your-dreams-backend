namespace AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;

public sealed class ImagenAtraccionService : IImagenAtraccionService
    {
        private readonly IImagenAtraccionDataService _data; public ImagenAtraccionService(IImagenAtraccionDataService data) => _data = data;
        public async Task<IReadOnlyList<ImagenAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(x => new ImagenAtraccionResponse { ImagenId = x.ImagenId, AtraccionId = x.AtraccionId, EsPrincipal = x.EsPrincipal, Orden = x.Orden, Estado = x.Estado }).ToList();
        public async Task<ImagenAtraccionResponse> AsociarAsync(AsociarImagenAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request); var x = await _data.GuardarAsync(new ImagenAtraccionDataModel { ImagenId = request.ImagenId, AtraccionId = request.AtraccionId, EsPrincipal = request.EsPrincipal, Orden = request.Orden, UsuarioIngreso = request.UsuarioIngreso, Estado = BusinessConstants.EstadoActivo }, cancellationToken); return new ImagenAtraccionResponse { ImagenId = x.ImagenId, AtraccionId = x.AtraccionId, EsPrincipal = x.EsPrincipal, Orden = x.Orden, Estado = x.Estado }; }
        public async Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        public async Task<BusinessOperationResult> RemoverAsync(int atraccionId, int imagenId, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(new AsociarImagenAtraccionRequest { AtraccionId = atraccionId, ImagenId = imagenId, Orden = 1 }); await _data.RemoverAsync(atraccionId, imagenId, cancellationToken); return BusinessOperationResult.Ok("Imagen desasociada."); }
        private static void Validate(AsociarImagenAtraccionRequest request) { var errors = new List<string>(); Support.Guard.Positive(request.AtraccionId, "AtraccionId", errors); Support.Guard.Positive(request.ImagenId, "ImagenId", errors); Support.Guard.Positive(request.Orden, "Orden", errors); Support.Guard.ThrowIfAny(errors); }
    }

