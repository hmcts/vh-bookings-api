using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Mappings
{
    public class ParticipantMap : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable(nameof(Participant));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasIndex(x => new {ParticipantId = x.PersonId, x.HearingId}).IsUnique();
            builder.Property(x => x.DisplayName);
            builder.Property(x => x.CaseRoleId).IsRequired(false);
            builder.Property(x => x.HearingRoleId);
            builder.Property(x => x.Discriminator);
            builder.HasOne(x => x.CaseRole).WithMany().HasForeignKey(x => x.CaseRoleId);
            builder.HasOne(x => x.HearingRole).WithMany().HasForeignKey(x => x.HearingRoleId);
            builder.HasOne<Hearing>("Hearing").WithMany("Participants").HasForeignKey(x => x.HearingId);
            builder.HasMany(x => x.LinkedParticipants).WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId);
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }

    public class RepresentativeMap : IEntityTypeConfiguration<Representative>
    {
        public void Configure(EntityTypeBuilder<Representative> builder)
        {
            builder.Property(x => x.Representee);
        }
    }
    
    public class IndividualMap : IEntityTypeConfiguration<Individual>
    {
        public void Configure(EntityTypeBuilder<Individual> builder)
        {
            // Method intentionally left empty.
        }
    }

    public class JudgeMap : IEntityTypeConfiguration<Judge>
    {
        public void Configure(EntityTypeBuilder<Judge> builder)
        {
            // Method intentionally left empty.
        }
    }
    
    public class StaffMemberMap : IEntityTypeConfiguration<StaffMember>
    {
        public void Configure(EntityTypeBuilder<StaffMember> builder)
        {
            // Method intentionally left empty.
        }
    }
    
    public class JudicialOfficeHolderMap : IEntityTypeConfiguration<JudicialOfficeHolder>
    {
        public void Configure(EntityTypeBuilder<JudicialOfficeHolder> builder)
        {
            // Method intentionally left empty.
        }
    }
}