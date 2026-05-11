using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ImagenAtraccionRepository : RepositoryBase<ImagenAtraccionEntity>, IImagenAtraccionRepository
{
    public ImagenAtraccionRepository(AtraccionesDbContext context) : base(context) { }

    public Task<ImagenAtraccionEntity?> ObtenerPorRelacionAsync(int atraccionId, int imagenId, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.img_id == imagenId, cancellationToken);
}
