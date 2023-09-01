namespace BookingsApi.DAL.Mappings
{
    public class VhoNonAvailabilityMap : IEntityTypeConfiguration<VhoNonAvailability>
    {
        public void Configure(EntityTypeBuilder<VhoNonAvailability> builder)
        {
            builder.ToTable("VhoNonAvailability");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.JusticeUser);
            
            builder.Property(x => x.StartTime).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(x => x.EndTime).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            
            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
        }
    }
}
