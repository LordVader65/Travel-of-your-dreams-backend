using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Identity;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<UsuarioEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Usuario);
        builder.HasKey(x => x.usu_id);
        builder.HasIndex(x => x.usu_guid).IsUnique();
        builder.HasIndex(x => x.usu_login).IsUnique();
        builder.Property(x => x.usu_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.usu_login).HasMaxLength(100).IsRequired();
        builder.Property(x => x.usu_password_hash).HasMaxLength(256).IsRequired();
        builder.Property(x => x.usu_fecha_registro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.usu_usuario_registro).HasMaxLength(100).IsRequired();
        builder.Property(x => x.usu_ip_registro).HasMaxLength(45).IsRequired();
        builder.Property(x => x.usu_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.usu_ip_mod).HasMaxLength(45);
        builder.Property(x => x.usu_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.usu_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.usu_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t => t.HasCheckConstraint("ck_usuario_estado", "usu_estado IN ('A','I')"));
    }
}
