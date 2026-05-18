using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsReservas.DataAccess.Entities;

namespace TravelDreams.MsReservas.DataAccess.Configurations;

public sealed class ReservaEstadoHistorialConfiguration : IEntityTypeConfiguration<ReservaEstadoHistorialEntity>
{
    public void Configure(EntityTypeBuilder<ReservaEstadoHistorialEntity> builder)
    {
        builder.ToTable("reserva_estado_historial");
        builder.HasKey(x => x.reh_id);
        builder.HasIndex(x => x.reh_guid).IsUnique();
        builder.HasIndex(x => x.rev_id);
        builder.Property(x => x.reh_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.reh_estado_anterior).HasMaxLength(20);
        builder.Property(x => x.reh_estado_nuevo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.reh_fecha_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.reh_usuario).HasMaxLength(100).IsRequired();
        builder.Property(x => x.reh_ip).HasMaxLength(45).IsRequired();
        builder.Property(x => x.reh_observacion).HasMaxLength(300);
        builder.HasOne(x => x.Reserva).WithMany(x => x.EstadoHistorial).HasForeignKey(x => x.rev_id);
    }
}
