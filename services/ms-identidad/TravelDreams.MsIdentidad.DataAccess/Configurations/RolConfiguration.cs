using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsIdentidad.DataAccess.Common;
using TravelDreams.MsIdentidad.DataAccess.Entities;

namespace TravelDreams.MsIdentidad.DataAccess.Configurations;

public sealed class RolConfiguration : IEntityTypeConfiguration<RolEntity>
{
    public void Configure(EntityTypeBuilder<RolEntity> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(x => x.rol_id);
        builder.HasIndex(x => x.rol_guid).IsUnique();
        builder.HasIndex(x => x.rol_descripcion).IsUnique();
        builder.Property(x => x.rol_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.rol_descripcion).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rol_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rol_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rol_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.rol_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.rol_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.rol_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t => t.HasCheckConstraint("ck_roles_estado", "rol_estado IN ('A','I')"));

        builder.HasData(
            new RolEntity { rol_id = 1, rol_guid = Guid.Parse("11111111-1111-1111-1111-111111111111"), rol_descripcion = DatabaseConstants.RolAdmin, rol_fecha_ingreso = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), rol_usuario_ingreso = "seed", rol_ip_ingreso = "migration", rol_estado = DatabaseConstants.EstadoActivo },
            new RolEntity { rol_id = 2, rol_guid = Guid.Parse("22222222-2222-2222-2222-222222222222"), rol_descripcion = DatabaseConstants.RolCliente, rol_fecha_ingreso = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), rol_usuario_ingreso = "seed", rol_ip_ingreso = "migration", rol_estado = DatabaseConstants.EstadoActivo });
    }
}
