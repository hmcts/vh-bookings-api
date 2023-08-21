namespace BookingsApi.DAL.Mappings
{
    public class JudiciaryParticipantMap : IEntityTypeConfiguration<JudiciaryParticipant>
    {
        public void Configure(EntityTypeBuilder<JudiciaryParticipant> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Hearing);
            builder.HasOne(x => x.JudiciaryPerson);
            
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
        }
    }
}
