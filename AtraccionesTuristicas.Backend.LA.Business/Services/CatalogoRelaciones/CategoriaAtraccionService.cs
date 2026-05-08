namespace AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;

public sealed class CategoriaAtraccionService : ICategoriaAtraccionService
    {
        private readonly ICategoriaAtraccionDataService _data; public CategoriaAtraccionService(ICategoriaAtraccionDataService data) => _data = data;
        public async Task<IReadOnlyList<CategoriaAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(x => new CategoriaAtraccionResponse { CategoriaId = x.CategoriaId, AtraccionId = x.AtraccionId, Estado = x.Estado }).ToList();
        public async Task<CategoriaAtraccionResponse> AsociarAsync(AsociarCategoriaAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var x = await _data.CrearAsync(new CategoriaAtraccionDataModel { CategoriaId = request.CategoriaId, AtraccionId = request.AtraccionId, UsuarioIngreso = request.UsuarioIngreso, Estado = BusinessConstants.EstadoActivo }, cancellationToken); return new CategoriaAtraccionResponse { CategoriaId = x.CategoriaId, AtraccionId = x.AtraccionId, Estado = x.Estado }; }
        public async Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
    }

