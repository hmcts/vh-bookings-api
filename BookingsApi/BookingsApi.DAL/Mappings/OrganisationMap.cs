namespace BookingsApi.DAL.Mappings
{
    public class OrganisationMap : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.ToTable(nameof(Organisation));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
        }
    }
}