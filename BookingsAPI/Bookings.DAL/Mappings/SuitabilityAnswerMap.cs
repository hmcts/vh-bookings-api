using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class SuitabilityAnswerMap : IEntityTypeConfiguration<SuitabilityAnswer>
    {
        public void Configure(EntityTypeBuilder<SuitabilityAnswer> builder)
        {
            builder.ToTable(nameof(SuitabilityAnswer));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ParticipantId).IsRequired();
            builder.Property(x => x.Key).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.HasOne(x => x.Participant).WithMany("SuitabilityAnswers").HasForeignKey(k => k.ParticipantId);
        }
    }
}
