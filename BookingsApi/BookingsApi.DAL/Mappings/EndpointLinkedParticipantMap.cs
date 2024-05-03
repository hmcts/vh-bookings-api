namespace BookingsApi.DAL.Mappings;

public class EndpointLinkedParticipantMap : IEntityTypeConfiguration<EndpointLinkedParticipant>
{
    public void Configure(EntityTypeBuilder<EndpointLinkedParticipant> builder)
    {
        builder.ToTable("EndpointLinkedParticipant");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EndpointId).IsRequired();
        builder.Property(x => x.ParticipantId).IsRequired();
        builder.HasOne(x => x.Endpoint).WithMany(x => x.EndpointLinkedParticipants).HasForeignKey(x => x.EndpointId);
        builder.HasOne(x => x.Participant).WithMany(x => x.EndpointLinkedParticipants).HasForeignKey(x => x.ParticipantId);
    }
}