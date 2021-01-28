using Bookings.Domain;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class LinkedParticipantMap : IEntityTypeConfiguration<LinkedParticipant>
    {
        public void Configure(EntityTypeBuilder<LinkedParticipant> builder)
        {
            builder.ToTable(nameof(LinkedParticipant));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.ParticipantId).IsRequired();
            builder.Property(x => x.LinkedParticipantId).IsRequired();
            builder.Property(x => x.Type);
            
            builder.HasOne<Participant>("Participant")
                .WithMany("LinkedParticipant")
                .HasForeignKey(x => x.ParticipantId);
            
            builder.HasOne<Participant>("Participant")
                .WithMany("LinkedParticipant")
                .HasForeignKey(x => x.LinkedParticipantId);
        }
    }
}