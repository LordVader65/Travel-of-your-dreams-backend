using TravelDreams.MsAtracciones.DataAccess.Common;
using TravelDreams.MsAtracciones.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TravelDreams.MsAtracciones.DataAccess.Configurations.Operacion;

public sealed class TicketConfiguration : IEntityTypeConfiguration<TicketEntity>
{
    public void Configure(EntityTypeBuilder<TicketEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Ticket);
        builder.HasKey(x => x.tck_id);
        builder.HasIndex(x => x.tck_guid).IsUnique();
        builder.HasIndex(x => x.at_id);
        builder.Property(x => x.tck_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.tck_titulo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.tck_precio).HasPrecision(10, 2);
        builder.Property(x => x.tck_moneda).HasMaxLength(3).IsFixedLength().HasDefaultValue(DatabaseConstants.MonedaDefault);
        builder.Property(x => x.tck_tipo_participante).HasMaxLength(30).HasDefaultValue("Adulto");
        builder.Property(x => x.tck_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.tck_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.tck_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.tck_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.tck_ip_mod).HasMaxLength(45);
        builder.Property(x => x.tck_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.tck_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.tck_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.Tickets).HasForeignKey(x => x.at_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_ticket_estado", "tck_estado IN ('A','I')");
            t.HasCheckConstraint("ck_ticket_precio", "tck_precio >= 0");
            t.HasCheckConstraint("ck_ticket_capacidad", "tck_capacidad_maxima > 0");
        });
    }
}
