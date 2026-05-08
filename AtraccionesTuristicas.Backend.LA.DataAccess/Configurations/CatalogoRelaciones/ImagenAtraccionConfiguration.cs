using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.CatalogoRelaciones;

public sealed class ImagenAtraccionConfiguration : IEntityTypeConfiguration<ImagenAtraccionEntity>
{
    public void Configure(EntityTypeBuilder<ImagenAtraccionEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.ImagenAtraccion);
        builder.HasKey(x => new { x.img_id, x.at_id });
        builder.HasIndex(x => new { x.at_id, x.ima_es_principal });
        builder.Property(x => x.ima_es_principal).HasDefaultValue(false);
        builder.Property(x => x.ima_orden).HasDefaultValue(0);
        builder.Property(x => x.ima_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.ima_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ima_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.ima_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Imagen).WithMany(x => x.ImagenAtracciones).HasForeignKey(x => x.img_id);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.ImagenAtracciones).HasForeignKey(x => x.at_id);
    }
}
