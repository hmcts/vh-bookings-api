using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.DAL.Mappings
{
    public class EndpointMap : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.ToTable("Endpoint");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.DisplayName).IsRequired();
            builder.HasIndex(x => x.Sip).IsUnique();
            builder.Property(x => x.Pin).IsRequired();
            builder.Property(x=> x.ExternalReferenceId);
            builder.Property(x => x.MeasuresExternalId);
            builder.HasOne<Hearing>("Hearing").WithMany("Endpoints").HasForeignKey(x => x.HearingId);
            builder.HasOne(x => x.DefenceAdvocate);
            builder.HasOne(x => x.InterpreterLanguage).WithMany().HasForeignKey(x => x.InterpreterLanguageId).IsRequired(false);
            
            builder.HasOne(participant => participant.Screening)
                .WithOne(screening => screening.Endpoint)
                .HasForeignKey<Screening>(screening => screening.EndpointId);
        }
    }
}