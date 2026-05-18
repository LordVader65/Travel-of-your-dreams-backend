using TravelDreams.MsAtracciones.DataAccess.Common;
using TravelDreams.MsAtracciones.DataAccess.Entities.CatalogoRelaciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TravelDreams.MsAtracciones.DataAccess.Configurations.CatalogoRelaciones;

public sealed class IdiomaAtraccionConfiguration : IEntityTypeConfiguration<IdiomaAtraccionEntity>
{
    public void Configure(EntityTypeBuilder<IdiomaAtraccionEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.IdiomaAtraccion);
        builder.HasKey(x => new { x.id_id, x.at_id });
        builder.Property(x => x.ia_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.ia_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ia_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.ia_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Idioma).WithMany(x => x.IdiomaAtracciones).HasForeignKey(x => x.id_id);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.IdiomaAtracciones).HasForeignKey(x => x.at_id);
    }
}
