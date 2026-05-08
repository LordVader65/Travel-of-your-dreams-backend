using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Auditoria;

public sealed class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditoriaLogEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.AuditoriaLog);
        builder.HasKey(x => x.log_id);
        builder.HasIndex(x => x.log_guid).IsUnique();
        builder.HasIndex(x => new { x.log_tabla, x.log_registro_id });
        builder.HasIndex(x => x.log_fecha_utc);
        builder.HasIndex(x => x.log_usuario);
        builder.Property(x => x.log_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.log_tabla).HasMaxLength(100).IsRequired();
        builder.Property(x => x.log_operacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.log_fecha_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.log_usuario).HasMaxLength(100).IsRequired();
        builder.Property(x => x.log_ip).HasMaxLength(45).IsRequired();
        builder.Property(x => x.log_origen_canal).HasMaxLength(200);
    }
}
