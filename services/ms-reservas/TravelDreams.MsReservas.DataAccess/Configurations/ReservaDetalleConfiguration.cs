using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsReservas.DataAccess.Common;
using TravelDreams.MsReservas.DataAccess.Entities;

namespace TravelDreams.MsReservas.DataAccess.Configurations;

public sealed class ReservaDetalleConfiguration : IEntityTypeConfiguration<ReservaDetalleEntity>
{
    public void Configure(EntityTypeBuilder<ReservaDetalleEntity> builder)
    {
        builder.ToTable("reserva_detalle");
        builder.HasKey(x => x.rdet_id);
        builder.HasIndex(x => x.rdet_guid).IsUnique();
        builder.HasIndex(x => new { x.rev_id, x.tck_guid }).IsUnique();
        builder.Property(x => x.rdet_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.rdet_ticket_titulo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.rdet_tipo_participante).HasMaxLength(30).IsRequired();
        builder.Property(x => x.rdet_precio_unit).HasPrecision(10, 2);
        builder.Property(x => x.rdet_subtotal).HasPrecision(10, 2);
        builder.Property(x => x.rdet_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rdet_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rdet_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.rdet_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Reserva).WithMany(x => x.Detalles).HasForeignKey(x => x.rev_id);
    }
}
