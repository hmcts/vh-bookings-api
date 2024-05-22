namespace BookingsApi.DAL.Mappings;

public class EndpointParticipantMap : IEntityTypeConfiguration<EndpointParticipant>
{
    public void Configure(EntityTypeBuilder<EndpointParticipant> builder)
    {
        builder.ToTable(nameof(EndpointParticipant));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.EndpointId).IsRequired();
        builder.Property(x => x.ParticipantId).IsRequired();
        builder.HasOne(x => x.Endpoint)
            .WithMany(x => x.EndpointParticipants)
            .HasForeignKey(x => x.EndpointId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Participant)
            .WithMany(x => x.EndpointLinkedParticipants)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}