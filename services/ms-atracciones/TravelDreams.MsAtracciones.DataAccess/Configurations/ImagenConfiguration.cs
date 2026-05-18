using TravelDreams.MsAtracciones.DataAccess.Common;
using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TravelDreams.MsAtracciones.DataAccess.Configurations.Catalogo;

public sealed class ImagenConfiguration : IEntityTypeConfiguration<ImagenEntity>
{
    public void Configure(EntityTypeBuilder<ImagenEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Imagen);
        builder.HasKey(x => x.img_id);
        builder.HasIndex(x => x.img_guid).IsUnique();
        builder.Property(x => x.img_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.img_url).HasMaxLength(500).IsRequired();
        builder.Property(x => x.img_descripcion).HasMaxLength(200);
        builder.Property(x => x.img_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.img_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.img_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.img_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.img_ip_mod).HasMaxLength(45);
        builder.Property(x => x.img_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.img_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.img_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t => t.HasCheckConstraint("ck_imagen_estado", "img_estado IN ('A','I')"));
    }
}
