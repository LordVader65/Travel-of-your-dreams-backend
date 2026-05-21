using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;
using TravelDreams.MsAtracciones.DataAccess.Entities.CatalogoRelaciones;
using TravelDreams.MsAtracciones.DataAccess.Entities.Operacion;

namespace TravelDreams.MsAtracciones.DataAccess.Context;

public sealed class AtraccionesDbContext : DbContext
{
    public AtraccionesDbContext(DbContextOptions<AtraccionesDbContext> options)
        : base(options)
    {
    }

    public DbSet<DestinoEntity> Destinos => Set<DestinoEntity>();
    public DbSet<CategoriaEntity> Categorias => Set<CategoriaEntity>();
    public DbSet<IdiomaEntity> Idiomas => Set<IdiomaEntity>();
    public DbSet<IncluyeEntity> Incluyes => Set<IncluyeEntity>();
    public DbSet<ImagenEntity> Imagenes => Set<ImagenEntity>();
    public DbSet<AtraccionEntity> Atracciones => Set<AtraccionEntity>();
    public DbSet<CategoriaAtraccionEntity> CategoriaAtracciones => Set<CategoriaAtraccionEntity>();
    public DbSet<IdiomaAtraccionEntity> IdiomaAtracciones => Set<IdiomaAtraccionEntity>();
    public DbSet<ImagenAtraccionEntity> ImagenAtracciones => Set<ImagenAtraccionEntity>();
    public DbSet<AtraccionIncluyeEntity> AtraccionIncluyes => Set<AtraccionIncluyeEntity>();
    public DbSet<TicketEntity> Tickets => Set<TicketEntity>();
    public DbSet<HorarioEntity> Horarios => Set<HorarioEntity>();
    public DbSet<HorarioReglaEntity> HorarioReglas => Set<HorarioReglaEntity>();
    public DbSet<ReseniaEntity> Resenias => Set<ReseniaEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtraccionesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
