using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.CatalogoRelaciones;

public sealed class CategoriaAtraccionConfiguration : IEntityTypeConfiguration<CategoriaAtraccionEntity>
{
    public void Configure(EntityTypeBuilder<CategoriaAtraccionEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.CategoriaAtraccion);
        builder.HasKey(x => new { x.cat_id, x.at_id });
        builder.Property(x => x.ca_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.ca_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ca_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.ca_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Categoria).WithMany(x => x.CategoriaAtracciones).HasForeignKey(x => x.cat_id);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.CategoriaAtracciones).HasForeignKey(x => x.at_id);
    }
}
