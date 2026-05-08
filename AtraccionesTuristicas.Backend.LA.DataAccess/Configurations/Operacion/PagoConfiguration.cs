using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Operacion;

public sealed class PagoConfiguration : IEntityTypeConfiguration<PagoEntity>
{
    public void Configure(EntityTypeBuilder<PagoEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Pagos);
        builder.HasKey(x => x.pag_id);
        builder.HasIndex(x => x.pag_guid).IsUnique();
        builder.HasIndex(x => x.rev_id);
        builder.HasIndex(x => x.pag_referencia).IsUnique();
        builder.Property(x => x.pag_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.pag_referencia).HasMaxLength(100);
        builder.Property(x => x.pag_metodo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.pag_monto).HasPrecision(10, 2);
        builder.Property(x => x.pag_moneda).HasMaxLength(3).IsFixedLength().HasDefaultValue(DatabaseConstants.MonedaDefault);
        builder.Property(x => x.pag_fecha_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.pag_estado).HasMaxLength(20).HasDefaultValue(DatabaseConstants.PagoPendiente);
        builder.Property(x => x.pag_origen_canal).HasMaxLength(50);
        builder.Property(x => x.pag_usuario_ingreso).HasMaxLength(100).IsRequired();
        builder.Property(x => x.pag_ip_ingreso).HasMaxLength(45).IsRequired();
        builder.Property(x => x.pag_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.pag_ip_mod).HasMaxLength(45);
        builder.HasOne(x => x.Reserva).WithMany(x => x.Pagos).HasForeignKey(x => x.rev_id);
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_pagos_estado", "pag_estado IN ('PENDIENTE','APROBADO','RECHAZADO')");
            t.HasCheckConstraint("ck_pagos_monto", "pag_monto >= 0");
        });
    }
}
