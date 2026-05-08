namespace AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;

public sealed class IdiomaAtraccionService : IIdiomaAtraccionService
    {
        private readonly IIdiomaAtraccionDataService _data; public IdiomaAtraccionService(IIdiomaAtraccionDataService data) => _data = data;
        public async Task<IReadOnlyList<IdiomaAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(x => new IdiomaAtraccionResponse { IdiomaId = x.IdiomaId, AtraccionId = x.AtraccionId, Estado = x.Estado }).ToList();
        public async Task<IdiomaAtraccionResponse> AsociarAsync(AsociarIdiomaAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var x = await _data.CrearAsync(new IdiomaAtraccionDataModel { IdiomaId = request.IdiomaId, AtraccionId = request.AtraccionId, UsuarioIngreso = request.UsuarioIngreso, Estado = BusinessConstants.EstadoActivo }, cancellationToken); return new IdiomaAtraccionResponse { IdiomaId = x.IdiomaId, AtraccionId = x.AtraccionId, Estado = x.Estado }; }
        public async Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
    }

