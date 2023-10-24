namespace BookingsApi.DAL.Mappings
{
    public class HearingVenueMap : IEntityTypeConfiguration<HearingVenue>
    {
        public void Configure(EntityTypeBuilder<HearingVenue> builder)
        {
            builder.ToTable(nameof(HearingVenue));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Name);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasIndex(x => x.VenueCode).IsUnique();
            builder.Property(x => x.IsScottish).HasDefaultValue(false);
            builder.Property(x => x.IsWorkAllocationEnabled).HasDefaultValue(true);
            builder.Property(x => x.ExpirationDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
        }
    }
}