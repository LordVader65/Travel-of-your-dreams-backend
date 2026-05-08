using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Cliente;

public sealed class DatosFacturacionConfiguration : IEntityTypeConfiguration<DatosFacturacionEntity>
{
    public void Configure(EntityTypeBuilder<DatosFacturacionEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.DatosFacturacion);
        builder.HasKey(x => x.dfac_id);
        builder.HasIndex(x => x.dfac_guid).IsUnique();
        builder.HasIndex(x => x.cli_id);
        builder.HasIndex(x => new { x.cli_id, x.dfac_numero_identificacion });
        builder.Property(x => x.dfac_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.dfac_tipo_identificacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.dfac_numero_identificacion).HasMaxLength(30).IsRequired();
        builder.Property(x => x.dfac_razon_social).HasMaxLength(200);
        builder.Property(x => x.dfac_nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.dfac_apellido).HasMaxLength(100);
        builder.Property(x => x.dfac_correo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.dfac_telefono).HasMaxLength(20);
        builder.Property(x => x.dfac_direccion).HasMaxLength(300);
        builder.Property(x => x.dfac_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.dfac_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.dfac_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.dfac_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.dfac_ip_mod).HasMaxLength(45);
        builder.Property(x => x.dfac_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.dfac_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.dfac_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Cliente).WithMany(x => x.DatosFacturacion).HasForeignKey(x => x.cli_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_dfac_correo", "dfac_correo LIKE '%@%.%'");
            t.HasCheckConstraint("ck_dfac_estado", "dfac_estado IN ('A','I')");
        });
    }
}
