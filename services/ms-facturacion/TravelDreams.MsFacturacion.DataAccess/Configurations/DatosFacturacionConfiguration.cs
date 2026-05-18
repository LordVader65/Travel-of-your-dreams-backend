using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsFacturacion.DataAccess.Common;
using TravelDreams.MsFacturacion.DataAccess.Entities;

namespace TravelDreams.MsFacturacion.DataAccess.Configurations;

public sealed class DatosFacturacionConfiguration : IEntityTypeConfiguration<DatosFacturacionEntity>
{
    public void Configure(EntityTypeBuilder<DatosFacturacionEntity> builder)
    {
        builder.ToTable("datos_facturacion");
        builder.HasKey(x => x.dfac_id);
        builder.HasIndex(x => x.dfac_guid).IsUnique();
        builder.HasIndex(x => x.cli_guid);
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
        builder.Property(x => x.dfac_row_version).HasDefaultValue(1).IsConcurrencyToken();
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_datos_facturacion_estado", "dfac_estado IN ('A','I')");
            t.HasCheckConstraint("ck_datos_facturacion_tipo_id", "dfac_tipo_identificacion IN ('CC','RUC','PASAPORTE','CEDULA','OTRO')");
        });
    }
}
