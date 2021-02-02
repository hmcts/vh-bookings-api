using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            builder.Property(x => x.CaseRoleId);
            builder.Property(x => x.HearingRoleId);

            builder.HasOne(x => x.CaseRole).WithMany().HasForeignKey(x => x.CaseRoleId);
            builder.HasOne(x => x.HearingRole).WithMany().HasForeignKey(x => x.HearingRoleId);

            builder.HasOne<Hearing>("Hearing").WithMany("Participants").HasForeignKey(x => x.HearingId);

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
    
    public class JudicialOfficeHolderMap : IEntityTypeConfiguration<JudicialOfficeHolder>
    {
        public void Configure(EntityTypeBuilder<JudicialOfficeHolder> builder)
        {
            // Method intentionally left empty.
        }
    }
}