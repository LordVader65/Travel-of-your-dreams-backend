using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Catalogo;

public sealed class IdiomaConfiguration : IEntityTypeConfiguration<IdiomaEntity>
{
    public void Configure(EntityTypeBuilder<IdiomaEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Idioma);
        builder.HasKey(x => x.id_id);
        builder.HasIndex(x => x.id_guid).IsUnique();
        builder.HasIndex(x => x.id_codigo).IsUnique();
        builder.HasIndex(x => x.id_descripcion).IsUnique();
        builder.Property(x => x.id_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.id_codigo).HasMaxLength(10).IsRequired();
        builder.Property(x => x.id_descripcion).HasMaxLength(80).IsRequired();
        builder.Property(x => x.id_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.id_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.id_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.id_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.id_ip_mod).HasMaxLength(45);
        builder.Property(x => x.id_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.id_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.id_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t => t.HasCheckConstraint("ck_idioma_estado", "id_estado IN ('A','I')"));
    }
}
