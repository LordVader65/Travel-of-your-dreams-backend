using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Operacion;

public sealed class ReservaDetalleConfiguration : IEntityTypeConfiguration<ReservaDetalleEntity>
{
    public void Configure(EntityTypeBuilder<ReservaDetalleEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.ReservaDetalle);
        builder.HasKey(x => x.rdet_id);
        builder.HasIndex(x => x.rdet_guid).IsUnique();
        builder.HasIndex(x => new { x.rev_id, x.tck_id }).IsUnique();
        builder.Property(x => x.rdet_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.rdet_precio_unit).HasPrecision(10, 2);
        builder.Property(x => x.rdet_subtotal).HasPrecision(10, 2);
        builder.Property(x => x.rdet_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rdet_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rdet_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.rdet_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.rdet_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.rdet_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Reserva).WithMany(x => x.Detalles).HasForeignKey(x => x.rev_id);
        builder.HasOne(x => x.Ticket).WithMany(x => x.ReservaDetalles).HasForeignKey(x => x.tck_id);
    }
}
