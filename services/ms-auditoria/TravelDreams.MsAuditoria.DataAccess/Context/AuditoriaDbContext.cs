using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAuditoria.DataAccess.Entities;

namespace TravelDreams.MsAuditoria.DataAccess.Context;

public sealed class AuditoriaDbContext : DbContext
{
    public AuditoriaDbContext(DbContextOptions<AuditoriaDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuditoriaLogEntity> AuditoriaLogs => Set<AuditoriaLogEntity>();
    public DbSet<EventoProcesadoEntity> EventosProcesados => Set<EventoProcesadoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditoriaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
