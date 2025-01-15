namespace BookingsApi.DAL.Mappings
{
    public class JudiciaryParticipantMap : IEntityTypeConfiguration<JudiciaryParticipant>
    {
        public void Configure(EntityTypeBuilder<JudiciaryParticipant> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasOne(x => x.Hearing);
            builder.HasOne(x => x.JudiciaryPerson);
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.HasOne(x => x.InterpreterLanguage).WithMany().HasForeignKey(x => x.InterpreterLanguageId).IsRequired(false);
        }
    }
}
