using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Catalogo;

public sealed class IncluyeConfiguration : IEntityTypeConfiguration<IncluyeEntity>
{
    public void Configure(EntityTypeBuilder<IncluyeEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Incluye);
        builder.HasKey(x => x.inc_id);
        builder.HasIndex(x => x.inc_guid).IsUnique();
        builder.Property(x => x.inc_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.inc_descripcion).HasMaxLength(200).IsRequired();
        builder.Property(x => x.inc_tipo).HasMaxLength(20).HasDefaultValue(DatabaseConstants.Incluye);
        builder.Property(x => x.inc_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_incluye_estado", "inc_estado IN ('A','I')");
            t.HasCheckConstraint("ck_incluye_tipo", "inc_tipo IN ('INCLUYE','NO_INCLUYE','ETIQUETA')");
        });
    }
}
