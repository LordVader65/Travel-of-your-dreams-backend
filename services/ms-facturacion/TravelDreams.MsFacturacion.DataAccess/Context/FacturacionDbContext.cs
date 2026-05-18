using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.DataAccess.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FacturacionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
