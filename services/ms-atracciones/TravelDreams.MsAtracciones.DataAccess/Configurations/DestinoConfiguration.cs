using TravelDreams.MsAtracciones.DataAccess.Common;
using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TravelDreams.MsAtracciones.DataAccess.Configurations.Catalogo;

public sealed class DestinoConfiguration : IEntityTypeConfiguration<DestinoEntity>
{
    public void Configure(EntityTypeBuilder<DestinoEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Destino);
        builder.HasKey(x => x.des_id);
        builder.HasIndex(x => x.des_guid).IsUnique();
        builder.HasIndex(x => x.des_pais);
        builder.Property(x => x.des_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.des_nombre).HasMaxLength(150).IsRequired();
        builder.Property(x => x.des_pais).HasMaxLength(100).IsRequired();
        builder.Property(x => x.des_imagen_url).HasMaxLength(500);
        builder.Property(x => x.des_fecha_ingreso).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.des_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.des_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.des_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.des_ip_mod).HasMaxLength(45);
        builder.Property(x => x.des_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.des_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.des_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.ToTable(t => t.HasCheckConstraint("ck_destino_estado", "des_estado IN ('A','I')"));
    }
}
