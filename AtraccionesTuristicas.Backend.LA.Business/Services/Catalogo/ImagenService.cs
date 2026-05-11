namespace AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;

public sealed class ImagenService : IImagenService
    {
        private readonly IImagenDataService _data; public ImagenService(IImagenDataService data) => _data = data;
        public async Task<IReadOnlyList<ImagenResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Imagen).ToList();
        public async Task<ImagenResponse> CrearAsync(CrearImagenRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request); return Map.Imagen(await _data.CrearAsync(Map.Imagen(request), cancellationToken)); }
        public async Task<ImagenResponse> ActualizarAsync(ActualizarImagenRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var m = Map.Imagen((CrearImagenRequest)request); m.Id = request.Id; m.Guid = request.Guid; m.Estado = request.Estado; return Map.Imagen(await _data.ActualizarAsync(m, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        private static void Validate(CrearImagenRequest request) { var errors = new List<string>(); Support.Guard.Required(request.Url, "Url", errors); Support.Guard.MaxLength(request.Url, 500, "Url", errors); Support.Guard.MaxLength(request.Descripcion, 200, "Descripcion", errors); Support.Guard.ThrowIfAny(errors); }
    }
