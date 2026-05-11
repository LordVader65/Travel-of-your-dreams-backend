using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Cliente;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<ClienteEntity>
{
    public void Configure(EntityTypeBuilder<ClienteEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Clientes);
        builder.HasKey(x => x.cli_id);
        builder.HasIndex(x => x.cli_guid).IsUnique();
        builder.HasIndex(x => x.cli_numero_identificacion).IsUnique();
        builder.Property(x => x.cli_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.cli_tipo_identificacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.cli_numero_identificacion).HasMaxLength(20).IsRequired();
        builder.Property(x => x.cli_nombres).HasMaxLength(100);
        builder.Property(x => x.cli_apellidos).HasMaxLength(100);
        builder.Property(x => x.cli_razon_social).HasMaxLength(200);
        builder.Property(x => x.cli_correo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.cli_telefono).HasMaxLength(10);
        builder.Property(x => x.cli_direccion).HasMaxLength(300);
        builder.Property(x => x.cli_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.cli_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.cli_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.cli_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.cli_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.cli_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.Property(x => x.cli_row_version).HasDefaultValue(1).IsConcurrencyToken();
        builder.HasOne(x => x.Usuario).WithMany(x => x.Clientes).HasForeignKey(x => x.usu_id).IsRequired(false);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_clientes_estado", "cli_estado IN ('A','I')");
            t.HasCheckConstraint("ck_clientes_tipo_id", "cli_tipo_identificacion IN ('CC','RUC','PASAPORTE','CEDULA','OTRO')");
        });
    }
}
