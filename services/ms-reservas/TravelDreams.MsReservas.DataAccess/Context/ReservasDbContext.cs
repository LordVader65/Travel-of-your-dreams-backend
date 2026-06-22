using Microsoft.EntityFrameworkCore;
using TravelDreams.MsReservas.DataAccess.Entities;
using TravelDreams.MsReservas.DataAccess.Entities.V3;

namespace TravelDreams.MsReservas.DataAccess.Context;

public sealed class ReservasDbContext : DbContext
{
    public ReservasDbContext(DbContextOptions<ReservasDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClienteEntity> Clientes => Set<ClienteEntity>();
    public DbSet<ReservaEntity> Reservas => Set<ReservaEntity>();
    public DbSet<ReservaDetalleEntity> ReservaDetalles => Set<ReservaDetalleEntity>();
    public DbSet<ReservaEstadoHistorialEntity> ReservaEstadoHistorial => Set<ReservaEstadoHistorialEntity>();
    public DbSet<MarketplaceReservaSolicitudV3Entity> MarketplaceReservaSolicitudesV3 => Set<MarketplaceReservaSolicitudV3Entity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservasDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
