using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Identity;

public sealed class RolConfiguration : IEntityTypeConfiguration<RolEntity>
{
    public void Configure(EntityTypeBuilder<RolEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Roles);
        builder.HasKey(x => x.rol_id);
        builder.HasIndex(x => x.rol_guid).IsUnique();
        builder.Property(x => x.rol_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.rol_descripcion).HasMaxLength(80).IsRequired();
        builder.Property(x => x.rol_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rol_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rol_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.rol_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.rol_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.rol_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t => t.HasCheckConstraint("ck_roles_estado", "rol_estado IN ('A','I')"));
    }
}
