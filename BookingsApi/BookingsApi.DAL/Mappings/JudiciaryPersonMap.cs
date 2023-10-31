namespace BookingsApi.DAL.Mappings
{
    public class JudiciaryPersonMap : IEntityTypeConfiguration<JudiciaryPerson>
    {
        public void Configure(EntityTypeBuilder<JudiciaryPerson> builder)
        {
            builder.ToTable(nameof(JudiciaryPerson));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasIndex(x => x.ExternalRefId).IsUnique();
            builder.Property(x => x.ExternalRefId);
            builder.HasIndex(x => x.PersonalCode).IsUnique();
            builder.Property(x => x.PersonalCode);
            builder.Property(x => x.Title);
            builder.Property(x => x.KnownAs);
            builder.Property(x => x.Surname);
            builder.Property(x => x.Fullname);
            builder.Property(x => x.PostNominals);
            builder.Property(x => x.Email);
            builder.Property(x => x.WorkPhone);
            
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}