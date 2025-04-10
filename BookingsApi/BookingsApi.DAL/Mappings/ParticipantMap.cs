﻿using BookingsApi.Domain.Participants;
using BookingsApi.Domain.SpecialMeasure;

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
            builder.Property<int?>("CaseRoleId").IsRequired(false);
            builder.Property(x => x.HearingRoleId);
            builder.Property(x=> x.ExternalReferenceId);
            builder.Property(x => x.MeasuresExternalId);
            builder.HasOne(x => x.HearingRole).WithMany().HasForeignKey(x => x.HearingRoleId);
            builder.HasOne<Hearing>("Hearing").WithMany("Participants").HasForeignKey(x => x.HearingId);
            builder.HasMany(x => x.LinkedParticipants).WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId);
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.HasOne(x => x.InterpreterLanguage).WithMany().HasForeignKey(x => x.InterpreterLanguageId).IsRequired(false);
            
            builder.Property(x => x.ScreeningId);
            builder.HasOne(participant => participant.Screening)
                .WithOne(screening => screening.Participant)
                .HasForeignKey<Screening>(screening => screening.ParticipantId);
            
            builder.HasOne(participant => participant.Endpoint)
                .WithMany(endpoint => endpoint.ParticipantsLinked)
                .HasForeignKey(p => p.EndpointId)
                .OnDelete(DeleteBehavior.NoAction); 
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
}