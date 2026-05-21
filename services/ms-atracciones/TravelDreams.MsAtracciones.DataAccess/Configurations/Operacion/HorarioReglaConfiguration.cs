using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsAtracciones.DataAccess.Common;
using TravelDreams.MsAtracciones.DataAccess.Entities.Operacion;

namespace TravelDreams.MsAtracciones.DataAccess.Configurations.Operacion;

public sealed class HorarioReglaConfiguration : IEntityTypeConfiguration<HorarioReglaEntity>
{
    public void Configure(EntityTypeBuilder<HorarioReglaEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.HorarioRegla);
        builder.HasKey(x => x.hreg_id);
        builder.HasIndex(x => x.hreg_guid).IsUnique();
        builder.HasIndex(x => new { x.at_id, x.hreg_hora_inicio, x.hreg_fecha_inicio, x.hreg_fecha_fin });
        builder.Property(x => x.hreg_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.hreg_dias_semana).HasMaxLength(20).HasDefaultValue("0,1,2,3,4,5,6");
        builder.Property(x => x.hreg_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.hreg_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.hreg_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.hreg_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.hreg_ip_mod).HasMaxLength(45);
        builder.Property(x => x.hreg_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.hreg_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.hreg_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.HorarioReglas).HasForeignKey(x => x.at_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_horario_regla_estado", "hreg_estado IN ('A','I')");
            t.HasCheckConstraint("ck_horario_regla_cupos", "hreg_cupos > 0");
            t.HasCheckConstraint("ck_horario_regla_rango", "hreg_fecha_fin >= hreg_fecha_inicio");
        });
    }
}
