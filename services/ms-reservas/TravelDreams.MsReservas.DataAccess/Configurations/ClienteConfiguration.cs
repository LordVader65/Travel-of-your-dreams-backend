using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsReservas.DataAccess.Common;
using TravelDreams.MsReservas.DataAccess.Entities;

namespace TravelDreams.MsReservas.DataAccess.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<ClienteEntity>
{
    public void Configure(EntityTypeBuilder<ClienteEntity> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(x => x.cli_id);
        builder.HasIndex(x => x.cli_guid).IsUnique();
        builder.HasIndex(x => x.cli_numero_identificacion).IsUnique();
        builder.Property(x => x.cli_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.usu_guid);
        builder.Property(x => x.cli_tipo_identificacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.cli_numero_identificacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.cli_nombres).HasMaxLength(100);
        builder.Property(x => x.cli_apellidos).HasMaxLength(100);
        builder.Property(x => x.cli_razon_social).HasMaxLength(200);
        builder.Property(x => x.cli_correo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.cli_telefono).HasMaxLength(20);
        builder.Property(x => x.cli_direccion).HasMaxLength(300);
        builder.Property(x => x.cli_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.cli_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.cli_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.cli_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.cli_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.cli_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.Property(x => x.cli_row_version).HasDefaultValue(1).IsConcurrencyToken();
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_clientes_estado", "cli_estado IN ('A','I')");
            t.HasCheckConstraint("ck_clientes_tipo_id", "cli_tipo_identificacion IN ('CC','RUC','PASAPORTE','CEDULA','OTRO')");
        });
    }
}
