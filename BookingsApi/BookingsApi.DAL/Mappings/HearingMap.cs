using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Mappings
{
    public class HearingMap : IEntityTypeConfiguration<Hearing>
    {
        public void Configure(EntityTypeBuilder<Hearing> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            
            builder.ToTable(nameof(Hearing));
            builder.Property(x => x.HearingMediumType).HasColumnName("HearingMediumId");
            builder.HasDiscriminator(x => x.HearingMediumType)
                .HasValue<VideoHearing>(HearingMediumType.FullyVideo);

            builder.Property(x => x.CaseTypeId).IsRequired();
            builder.Property(x => x.HearingTypeId);
            builder.Property<string>("HearingVenueName").HasMaxLength(450);
            builder.Property(x => x.ScheduledDateTime).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.ScheduledDuration);
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.Status).HasColumnName("HearingStatusId");
            builder.Property<bool>("QuestionnaireNotRequired").HasDefaultValue(false);
            builder.Property(x => x.CancelReason).HasMaxLength(255);
            builder.Property(x => x.SourceId);
            builder.Ignore(x => x.IsFirstDayOfMultiDayHearing);

            builder.HasMany<HearingCase>("HearingCases").WithOne(x => x.Hearing).HasForeignKey(x => x.HearingId);
            builder.HasMany<Allocation>(nameof(Hearing.Allocations)).WithOne(x => x.Hearing).HasForeignKey(x => x.HearingId);
            builder.HasMany<Endpoint>("Endpoints").WithOne("Hearing").HasForeignKey(x => x.HearingId);
            builder.HasMany<Participant>("Participants").WithOne("Hearing").HasForeignKey(x => x.HearingId);
            builder.HasMany<JudiciaryParticipant>("JudiciaryParticipants").WithOne("Hearing").HasForeignKey(x => x.HearingId);

            builder.HasOne(x => x.CaseType).WithMany().HasForeignKey(x => x.CaseTypeId).IsRequired();
            builder.HasOne(x => x.HearingType).WithMany().HasForeignKey(x => x.HearingTypeId);
            builder.HasOne(x => x.HearingVenue).WithMany().HasForeignKey(x => x.HearingVenueId);
        }
    }
}