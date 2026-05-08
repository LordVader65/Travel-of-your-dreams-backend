using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Operacion;

public sealed class HorarioConfiguration : IEntityTypeConfiguration<HorarioEntity>
{
    public void Configure(EntityTypeBuilder<HorarioEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Horario);
        builder.HasKey(x => x.hor_id);
        builder.HasIndex(x => x.hor_guid).IsUnique();
        builder.HasIndex(x => new { x.at_id, x.hor_fecha, x.hor_hora_inicio }).IsUnique();
        builder.Property(x => x.hor_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.hor_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.hor_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.hor_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.hor_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.hor_ip_mod).HasMaxLength(45);
        builder.Property(x => x.hor_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.hor_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.hor_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.Horarios).HasForeignKey(x => x.at_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_horario_estado", "hor_estado IN ('A','I')");
            t.HasCheckConstraint("ck_horario_cupos", "hor_cupos_disponibles >= 0");
        });
    }
}
