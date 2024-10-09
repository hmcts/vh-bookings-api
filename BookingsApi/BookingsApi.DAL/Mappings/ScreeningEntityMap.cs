using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.DAL.Mappings;

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