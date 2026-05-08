namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.CatalogoRelaciones;

public interface ICategoriaAtraccionService { Task<IReadOnlyList<CategoriaAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<CategoriaAtraccionResponse> AsociarAsync(AsociarCategoriaAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

