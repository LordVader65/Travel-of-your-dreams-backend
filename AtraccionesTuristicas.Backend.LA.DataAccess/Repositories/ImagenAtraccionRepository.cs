using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ImagenAtraccionRepository : RepositoryBase<ImagenAtraccionEntity>, IImagenAtraccionRepository
{
    public ImagenAtraccionRepository(AtraccionesDbContext context) : base(context) { }
}
