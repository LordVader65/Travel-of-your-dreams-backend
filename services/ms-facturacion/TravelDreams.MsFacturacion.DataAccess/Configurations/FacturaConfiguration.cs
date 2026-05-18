using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsFacturacion.DataAccess.Common;
using TravelDreams.MsFacturacion.DataAccess.Entities;

namespace TravelDreams.MsFacturacion.DataAccess.Configurations;

public sealed class FacturaConfiguration : IEntityTypeConfiguration<FacturaEntity>
{
    public void Configure(EntityTypeBuilder<FacturaEntity> builder)
    {
        builder.ToTable("facturas");
        builder.HasKey(x => x.fac_id);
        builder.HasIndex(x => x.fac_guid).IsUnique();
        builder.HasIndex(x => x.fac_numero).IsUnique();
        builder.HasIndex(x => x.rev_guid).IsUnique();
        builder.HasIndex(x => x.cli_guid);
        builder.HasIndex(x => x.pag_id).IsUnique();
        builder.Property(x => x.fac_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.fac_numero).HasMaxLength(50).IsRequired();
        builder.Property(x => x.fac_fecha_emision_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.fac_subtotal).HasColumnType("numeric(12,2)").IsRequired();
        builder.Property(x => x.fac_valor_iva).HasColumnType("numeric(12,2)").IsRequired();
        builder.Property(x => x.fac_total).HasColumnType("numeric(12,2)").IsRequired();
        builder.Property(x => x.fac_moneda).HasMaxLength(3).HasDefaultValue(DatabaseConstants.MonedaDefault);
        builder.Property(x => x.fac_observacion).HasMaxLength(500);
        builder.Property(x => x.fac_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.fac_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.fac_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.fac_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.fac_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.Property(x => x.fac_row_version).HasDefaultValue(1).IsConcurrencyToken();
        builder.HasOne(x => x.Pago).WithOne(x => x.Factura).HasForeignKey<FacturaEntity>(x => x.pag_id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DatosFacturacion).WithMany(x => x.Facturas).HasForeignKey(x => x.dfac_id).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_facturas_estado", "fac_estado IN ('A','I')");
            t.HasCheckConstraint("ck_facturas_subtotal", "fac_subtotal >= 0");
            t.HasCheckConstraint("ck_facturas_iva", "fac_valor_iva >= 0");
            t.HasCheckConstraint("ck_facturas_total", "fac_total >= 0");
        });
    }
}
