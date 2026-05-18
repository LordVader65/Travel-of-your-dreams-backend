using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsIdentidad.DataAccess.Common;
using TravelDreams.MsIdentidad.DataAccess.Entities;

namespace TravelDreams.MsIdentidad.DataAccess.Configurations;

public sealed class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRolEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioRolEntity> builder)
    {
        builder.ToTable("usuarioxroles");
        builder.HasKey(x => x.usu_rol_id);
        builder.HasIndex(x => new { x.usu_id, x.rol_id }).IsUnique();
        builder.Property(x => x.usu_rol_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Usuario).WithMany(x => x.UsuarioRoles).HasForeignKey(x => x.usu_id).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Rol).WithMany(x => x.UsuarioRoles).HasForeignKey(x => x.rol_id).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("ck_usuarioxroles_estado", "usu_rol_estado IN ('A','I')"));
    }
}
