using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.DataAccess.Entities;
using TravelDreams.MsFacturacion.DataAccess.Entities.V3;

namespace TravelDreams.MsFacturacion.DataAccess.Context;

public sealed class FacturacionDbContext : DbContext
{
    public FacturacionDbContext(DbContextOptions<FacturacionDbContext> options)
        : base(options)
    {
    }

    public DbSet<DatosFacturacionEntity> DatosFacturacion => Set<DatosFacturacionEntity>();
    public DbSet<PagoEntity> Pagos => Set<PagoEntity>();
    public DbSet<FacturaEntity> Facturas => Set<FacturaEntity>();
    public DbSet<MarketplacePagoSolicitudV3Entity> MarketplacePagoSolicitudesV3 => Set<MarketplacePagoSolicitudV3Entity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FacturacionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
