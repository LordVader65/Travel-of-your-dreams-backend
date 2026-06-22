using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsFacturacion.DataAccess.Entities.V3;

namespace TravelDreams.MsFacturacion.DataAccess.Configurations.V3;

public sealed class MarketplacePagoSolicitudV3Configuration : IEntityTypeConfiguration<MarketplacePagoSolicitudV3Entity>
{
    public void Configure(EntityTypeBuilder<MarketplacePagoSolicitudV3Entity> builder)
    {
        builder.ToTable("facturacion_solicitudes_v3");
        builder.HasKey(x => x.fsol_id);
        builder.HasIndex(x => x.fsol_correlation_id).IsUnique();
        builder.HasIndex(x => x.cli_guid);
        builder.HasIndex(x => x.rev_guid);
        builder.HasIndex(x => x.fsol_estado);
        builder.Property(x => x.fsol_estado).HasMaxLength(30).IsRequired();
        builder.Property(x => x.fac_numero).HasMaxLength(50);
        builder.Property(x => x.fsol_error).HasMaxLength(500);
        builder.Property(x => x.fsol_payload_json).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.fsol_created_at_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.fsol_updated_at_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
