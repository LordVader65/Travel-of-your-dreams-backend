using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Catalogo;

public sealed class CategoriaConfiguration : IEntityTypeConfiguration<CategoriaEntity>
{
    public void Configure(EntityTypeBuilder<CategoriaEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Categoria);
        builder.HasKey(x => x.cat_id);
        builder.HasIndex(x => x.cat_guid).IsUnique();
        builder.HasIndex(x => x.cat_tagname);
        builder.Property(x => x.cat_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.cat_nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.cat_tagname).HasMaxLength(80);
        builder.Property(x => x.cat_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.cat_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.cat_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.cat_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.cat_ip_mod).HasMaxLength(45);
        builder.Property(x => x.cat_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.cat_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.cat_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.cat_parent_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_categoria_estado", "cat_estado IN ('A','I')");
            t.HasCheckConstraint("ck_categoria_selfref", "cat_parent_id IS NULL OR cat_parent_id <> cat_id");
        });
    }
}
