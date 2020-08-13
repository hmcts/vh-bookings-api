using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
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
            builder.Property(x => x.HearingTypeId).IsRequired();
            builder.Property(x => x.HearingVenueName);
            builder.Property(x => x.ScheduledDateTime);
            builder.Property(x => x.ScheduledDuration);
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.Status).HasColumnName("HearingStatusId");
            builder.Property(x => x.QuestionnaireNotRequired);
            builder.Property(x => x.CancelReason).HasMaxLength(255);

            builder.HasMany<HearingCase>("HearingCases").WithOne(x => x.Hearing).HasForeignKey(x => x.HearingId);
            builder.HasMany<Endpoint>("Endpoints").WithOne(x => x.Hearing).HasForeignKey(x => x.HearingId);
            builder.HasMany<Participant>("Participants").WithOne("Hearing").HasForeignKey(x => x.HearingId);

            builder.HasOne(x => x.CaseType).WithMany().HasForeignKey(x => x.CaseTypeId).IsRequired();
            builder.HasOne(x => x.HearingType).WithMany().HasForeignKey(x => x.HearingTypeId).IsRequired();
            builder.HasOne(x => x.HearingVenue).WithMany().HasForeignKey(x => x.HearingVenueName);
        }
    }
}