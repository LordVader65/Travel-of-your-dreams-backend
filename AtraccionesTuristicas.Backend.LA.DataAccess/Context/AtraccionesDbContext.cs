using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Context;

public sealed class AtraccionesDbContext : DbContext
{
    public AtraccionesDbContext(DbContextOptions<AtraccionesDbContext> options)
        : base(options)
    {
    }

    public DbSet<RolEntity> Roles => Set<RolEntity>();
    public DbSet<UsuarioEntity> Usuarios => Set<UsuarioEntity>();
    public DbSet<UsuarioRolEntity> UsuarioRoles => Set<UsuarioRolEntity>();
    public DbSet<ClienteEntity> Clientes => Set<ClienteEntity>();
    public DbSet<DatosFacturacionEntity> DatosFacturacion => Set<DatosFacturacionEntity>();
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
    public DbSet<ReservaEntity> Reservas => Set<ReservaEntity>();
    public DbSet<ReservaDetalleEntity> ReservaDetalles => Set<ReservaDetalleEntity>();
    public DbSet<ReservaEstadoHistorialEntity> ReservaEstadoHistorial => Set<ReservaEstadoHistorialEntity>();
    public DbSet<PagoEntity> Pagos => Set<PagoEntity>();
    public DbSet<FacturaEntity> Facturas => Set<FacturaEntity>();
    public DbSet<ReseniaEntity> Resenias => Set<ReseniaEntity>();
    public DbSet<AuditoriaLogEntity> AuditoriaLogs => Set<AuditoriaLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtraccionesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
