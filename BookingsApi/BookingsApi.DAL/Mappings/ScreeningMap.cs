

using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.DAL.Mappings;

public class ScreeningMap : IEntityTypeConfiguration<Screening>
{
    public void Configure(EntityTypeBuilder<Screening> builder)
    {
        builder.ToTable(nameof(Screening));
        builder.HasKey(x => x.Id);
        
        builder.HasMany(x => x.ScreeningEntities)
            .WithOne(x => x.Screening)
            .HasForeignKey(x => x.ScreeningId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(screening => screening.Participant)
            .WithOne(participant => participant.Screening)
            .HasForeignKey<Screening>(screening => screening.ParticipantId);
        
        builder.HasOne(screening => screening.Endpoint)
            .WithOne(participant => participant.Screening)
            .HasForeignKey<Screening>(screening => screening.EndpointId);
    }
}

public class ScreeningEntityMap : IEntityTypeConfiguration<ScreeningEntity>
{
    public void Configure(EntityTypeBuilder<ScreeningEntity> builder)
    {
        builder.ToTable(nameof(ScreeningEntity));

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Screening)
            .WithMany(x => x.ScreeningEntities)
            .HasForeignKey(x => x.ScreeningId);
    }
}