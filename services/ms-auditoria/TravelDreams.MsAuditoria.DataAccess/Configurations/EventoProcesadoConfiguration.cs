using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsAuditoria.DataAccess.Entities;

namespace TravelDreams.MsAuditoria.DataAccess.Configurations;

public sealed class EventoProcesadoConfiguration : IEntityTypeConfiguration<EventoProcesadoEntity>
{
    public void Configure(EntityTypeBuilder<EventoProcesadoEntity> builder)
    {
        builder.ToTable("eventos_procesados");
        builder.HasKey(x => x.ep_id);
        builder.HasIndex(x => x.ep_evento_id).IsUnique();
        builder.HasIndex(x => x.ep_tipo);
        builder.HasIndex(x => x.ep_origen_servicio);
        builder.Property(x => x.ep_tipo).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ep_origen_servicio).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ep_fecha_procesado_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.ep_correlation_id).HasMaxLength(100);
    }
}
