using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.CatalogoRelaciones;

public sealed class AtraccionIncluyeConfiguration : IEntityTypeConfiguration<AtraccionIncluyeEntity>
{
    public void Configure(EntityTypeBuilder<AtraccionIncluyeEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.AtraccionIncluye);
        builder.HasKey(x => new { x.inc_id, x.at_id });
        builder.Property(x => x.ai_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.ai_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ai_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.ai_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Incluye).WithMany(x => x.AtraccionIncluyes).HasForeignKey(x => x.inc_id);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.AtraccionIncluyes).HasForeignKey(x => x.at_id);
    }
}
