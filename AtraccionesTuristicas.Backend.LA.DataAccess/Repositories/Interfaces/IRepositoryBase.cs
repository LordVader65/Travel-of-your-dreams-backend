namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IRepositoryBase<TEntity>
    where TEntity : class
{
    Task<TEntity?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListarAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Actualizar(TEntity entity);
    void Remover(TEntity entity);
}
