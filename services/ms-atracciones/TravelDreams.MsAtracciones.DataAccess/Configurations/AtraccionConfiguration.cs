using TravelDreams.MsAtracciones.DataAccess.Common;
using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TravelDreams.MsAtracciones.DataAccess.Configurations.Catalogo;

public sealed class AtraccionConfiguration : IEntityTypeConfiguration<AtraccionEntity>
{
    public void Configure(EntityTypeBuilder<AtraccionEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Atraccion);
        builder.HasKey(x => x.at_id);
        builder.HasIndex(x => x.at_guid).IsUnique();
        builder.HasIndex(x => x.des_id);
        builder.HasIndex(x => x.at_nombre);
        builder.Property(x => x.at_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.at_num_establecimiento).HasMaxLength(30);
        builder.Property(x => x.at_nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.at_descripcion).HasMaxLength(2000);
        builder.Property(x => x.at_total_resenias).HasDefaultValue(0);
        builder.Property(x => x.at_direccion).HasMaxLength(300);
        builder.Property(x => x.at_punto_encuentro).HasMaxLength(300);
        builder.Property(x => x.at_precio_referencia).HasPrecision(10, 2);
        builder.Property(x => x.at_incluye_acompaniante).HasDefaultValue(false);
        builder.Property(x => x.at_incluye_transporte).HasDefaultValue(false);
        builder.Property(x => x.at_disponible).HasDefaultValue(true);
        builder.Property(x => x.at_free_cancellation).HasDefaultValue(false);
        builder.Property(x => x.at_skip_the_line).HasDefaultValue(false);
        builder.Property(x => x.at_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.at_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.at_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.at_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.at_ip_mod).HasMaxLength(45);
        builder.Property(x => x.at_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.at_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.at_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Destino).WithMany(x => x.Atracciones).HasForeignKey(x => x.des_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_atraccion_estado", "at_estado IN ('A','I')");
            t.HasCheckConstraint("ck_atraccion_precio", "at_precio_referencia IS NULL OR at_precio_referencia >= 0");
            t.HasCheckConstraint("ck_atraccion_duracion", "at_duracion_minutos IS NULL OR at_duracion_minutos > 0");
            t.HasCheckConstraint("ck_atraccion_resenias", "at_total_resenias >= 0");
        });
    }
}
