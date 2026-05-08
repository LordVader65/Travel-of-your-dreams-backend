namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.CatalogoRelaciones;

public interface IAtraccionIncluyeService { Task<IReadOnlyList<AtraccionIncluyeResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<AtraccionIncluyeResponse> AsociarAsync(AsociarIncluyeAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

