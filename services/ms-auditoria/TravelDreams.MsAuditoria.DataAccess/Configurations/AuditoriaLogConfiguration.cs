using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsAuditoria.DataAccess.Entities;

namespace TravelDreams.MsAuditoria.DataAccess.Configurations;

public sealed class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditoriaLogEntity> builder)
    {
        builder.ToTable("auditoria_log");
        builder.HasKey(x => x.log_id);
        builder.HasIndex(x => x.log_guid).IsUnique();
        builder.HasIndex(x => x.evento_id).IsUnique();
        builder.HasIndex(x => new { x.log_servicio, x.log_tabla, x.log_registro_id });
        builder.HasIndex(x => x.log_fecha_utc);
        builder.HasIndex(x => x.log_usuario);
        builder.HasIndex(x => x.log_correlation_id);
        builder.Property(x => x.log_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.log_servicio).HasMaxLength(100).IsRequired();
        builder.Property(x => x.log_tabla).HasMaxLength(100).IsRequired();
        builder.Property(x => x.log_operacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.log_fecha_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.log_usuario).HasMaxLength(100).IsRequired();
        builder.Property(x => x.log_ip).HasMaxLength(45).IsRequired();
        builder.Property(x => x.log_origen_canal).HasMaxLength(200);
        builder.Property(x => x.log_correlation_id).HasMaxLength(100);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_auditoria_log_operacion", "log_operacion IN ('INSERT','UPDATE','DELETE','LOGIN','LOGOUT','BUSINESS_EVENT')");
            t.HasCheckConstraint("ck_auditoria_log_insert", "log_operacion <> 'INSERT' OR log_datos_anteriores IS NULL");
            t.HasCheckConstraint("ck_auditoria_log_delete", "log_operacion <> 'DELETE' OR log_datos_nuevos IS NULL");
        });
    }
}
