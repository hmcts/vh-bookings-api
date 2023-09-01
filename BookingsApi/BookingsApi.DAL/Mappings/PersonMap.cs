namespace BookingsApi.DAL.Mappings
{
    public class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable(nameof(Person));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Title);
            builder.Property(x => x.FirstName);
            builder.Property(x => x.MiddleNames);
            builder.Property(x => x.LastName);
            builder.Property(x => x.TelephoneNumber);
            builder.HasIndex(x => x.ContactEmail).IsUnique();
            builder.HasIndex(x => x.Username).IsUnique();

            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder.Property("OrganisationId").IsRequired(false);
        }
    }
}