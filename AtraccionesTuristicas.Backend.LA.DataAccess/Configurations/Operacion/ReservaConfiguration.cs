using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Operacion;

public sealed class ReservaConfiguration : IEntityTypeConfiguration<ReservaEntity>
{
    public void Configure(EntityTypeBuilder<ReservaEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Reservas);
        builder.HasKey(x => x.rev_id);
        builder.HasIndex(x => x.rev_guid).IsUnique();
        builder.HasIndex(x => x.rev_codigo).IsUnique();
        builder.HasIndex(x => x.cli_id);
        builder.HasIndex(x => x.hor_id);
        builder.Property(x => x.rev_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.rev_codigo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.rev_fecha_reserva_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rev_subtotal).HasPrecision(10, 2);
        builder.Property(x => x.rev_valor_iva).HasPrecision(10, 2);
        builder.Property(x => x.rev_total).HasPrecision(10, 2);
        builder.Property(x => x.rev_moneda).HasMaxLength(3).IsFixedLength().HasDefaultValue(DatabaseConstants.MonedaDefault);
        builder.Property(x => x.rev_origen_canal).HasMaxLength(50);
        builder.Property(x => x.rev_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rev_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.rev_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.rev_ip_mod).HasMaxLength(45);
        builder.Property(x => x.rev_usuario_cancelacion).HasMaxLength(100);
        builder.Property(x => x.rev_ip_cancelacion).HasMaxLength(45);
        builder.Property(x => x.rev_motivo_cancelacion).HasMaxLength(300);
        builder.Property(x => x.rev_estado).HasMaxLength(20).HasDefaultValue(DatabaseConstants.ReservaPendiente);
        builder.HasOne(x => x.Cliente).WithMany(x => x.Reservas).HasForeignKey(x => x.cli_id);
        builder.HasOne(x => x.Horario).WithMany(x => x.Reservas).HasForeignKey(x => x.hor_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_reservas_estado", "rev_estado IN ('PENDIENTE','PAGADA','CONFIRMADA','CANCELADA','EXPIRADA','USADA','NO_SHOW')");
            t.HasCheckConstraint("ck_reservas_subtotal", "rev_subtotal >= 0");
            t.HasCheckConstraint("ck_reservas_iva", "rev_valor_iva >= 0");
            t.HasCheckConstraint("ck_reservas_total", "rev_total >= 0");
        });
    }
}
