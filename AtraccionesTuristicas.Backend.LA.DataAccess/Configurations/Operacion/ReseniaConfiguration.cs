using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Configurations.Operacion;

public sealed class ReseniaConfiguration : IEntityTypeConfiguration<ReseniaEntity>
{
    public void Configure(EntityTypeBuilder<ReseniaEntity> builder)
    {
        builder.ToTable(DatabaseConstants.Tables.Resenia);
        builder.HasKey(x => x.rsn_id);
        builder.HasIndex(x => x.rsn_guid).IsUnique();
        builder.HasIndex(x => x.rev_id).IsUnique();
        builder.HasIndex(x => x.at_id);
        builder.Property(x => x.rsn_guid).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.rsn_comentario).HasMaxLength(1000);
        builder.Property(x => x.rsn_fecha_creacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.rsn_usuario_creacion).HasMaxLength(100).IsRequired();
        builder.Property(x => x.rsn_ip_creacion).HasMaxLength(45).IsRequired();
        builder.Property(x => x.rsn_usuario_mod).HasMaxLength(100);
        builder.Property(x => x.rsn_ip_mod).HasMaxLength(45);
        builder.Property(x => x.rsn_usuario_eliminacion).HasMaxLength(100);
        builder.Property(x => x.rsn_ip_eliminacion).HasMaxLength(45);
        builder.Property(x => x.rsn_estado).HasMaxLength(1).IsFixedLength().HasDefaultValue(DatabaseConstants.EstadoActivo);
        builder.HasOne(x => x.Atraccion).WithMany(x => x.Resenias).HasForeignKey(x => x.at_id);
        builder.HasOne(x => x.Reserva).WithOne(x => x.Resenia).HasForeignKey<ReseniaEntity>(x => x.rev_id);
        builder.ToTable(t => t.HasCheckConstraint("ck_resenia_rating", "rsn_rating BETWEEN 1 AND 5"));
    }
}
