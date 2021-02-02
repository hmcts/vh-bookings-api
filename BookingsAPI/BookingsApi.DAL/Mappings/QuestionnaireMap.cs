using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class QuestionnaireMap : IEntityTypeConfiguration<Questionnaire>
    {
        public void Configure(EntityTypeBuilder<Questionnaire> builder)
        {
            builder.ToTable(nameof(Questionnaire));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ParticipantId).IsRequired();
            builder.HasMany<SuitabilityAnswer>("SuitabilityAnswers").WithOne("Questionnaire").HasForeignKey(x => x.QuestionnaireId);
        }
    }
}
