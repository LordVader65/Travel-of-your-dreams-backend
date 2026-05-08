using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Identity;

public sealed class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRolEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioRolEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.UsuarioRoles);
        builder.HasKey(x => x.usu_rol_id);
        builder.HasIndex(x => new { x.usu_id, x.rol_id }).IsUnique();
        builder.Property(x => x.usu_rol_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Usuario).WithMany(x => x.UsuarioRoles).HasForeignKey(x => x.usu_id);
        builder.HasOne(x => x.Rol).WithMany(x => x.UsuarioRoles).HasForeignKey(x => x.rol_id);
        builder.ToTable(t => t.HasCheckConstraint("ck_usuarioxroles_estado", "usu_rol_estado IN ('A','I')"));
    }
}
