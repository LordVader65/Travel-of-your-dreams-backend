using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsFacturacion.DataAccess.Common;
using TravelDreams.MsFacturacion.DataAccess.Entities;

namespace TravelDreams.MsFacturacion.DataAccess.Configurations;

public sealed class PagoConfiguration : IEntityTypeConfiguration<PagoEntity>
{
    public void Configure(EntityTypeBuilder<PagoEntity> builder)
    {
        builder.ToTable("pagos");
        builder.HasKey(x => x.pag_id);
        builder.HasIndex(x => x.pag_guid).IsUnique();
        builder.HasIndex(x => x.pag_referencia).IsUnique();
        builder.HasIndex(x => x.rev_guid);
        builder.HasIndex(x => x.cli_guid);
        builder.Property(x => x.pag_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.pag_monto).HasColumnType("numeric(12,2)").IsRequired();
        builder.Property(x => x.pag_moneda).HasMaxLength(3).HasDefaultValue(DatabaseConstants.MonedaDefault);
        builder.Property(x => x.pag_metodo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.pag_referencia).HasMaxLength(100).IsRequired();
        builder.Property(x => x.pag_fecha_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.pag_origen_canal).HasMaxLength(50);
        builder.Property(x => x.pag_estado).HasMaxLength(20).HasDefaultValue(DatabaseConstants.PagoAprobado);
        builder.Property(x => x.pag_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.pag_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.pag_observacion).HasMaxLength(500);
        builder.Property(x => x.pag_row_version).HasDefaultValue(1).IsConcurrencyToken();
        builder.HasOne(x => x.DatosFacturacion).WithMany(x => x.Pagos).HasForeignKey(x => x.dfac_id).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_pagos_estado", "pag_estado IN ('PENDIENTE','APROBADO','RECHAZADO')");
            t.HasCheckConstraint("ck_pagos_monto", "pag_monto >= 0");
        });
    }
}
