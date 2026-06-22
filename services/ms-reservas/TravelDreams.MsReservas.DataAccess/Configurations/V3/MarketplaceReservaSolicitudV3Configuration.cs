using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelDreams.MsReservas.DataAccess.Entities.V3;

namespace TravelDreams.MsReservas.DataAccess.Configurations.V3;

public sealed class MarketplaceReservaSolicitudV3Configuration : IEntityTypeConfiguration<MarketplaceReservaSolicitudV3Entity>
{
    public void Configure(EntityTypeBuilder<MarketplaceReservaSolicitudV3Entity> builder)
    {
        builder.ToTable("reservas_solicitudes_v3");
        builder.HasKey(x => x.rsol_id);
        builder.HasIndex(x => x.rsol_correlation_id).IsUnique();
        builder.HasIndex(x => x.cli_guid);
        builder.HasIndex(x => x.rsol_estado);
        builder.Property(x => x.rsol_estado).HasMaxLength(30).IsRequired();
        builder.Property(x => x.rev_codigo).HasMaxLength(20);
        builder.Property(x => x.rsol_error).HasMaxLength(500);
        builder.Property(x => x.rsol_payload_json).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.rsol_created_at_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rsol_updated_at_utc).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
