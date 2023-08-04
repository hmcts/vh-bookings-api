namespace BookingsApi.DAL.Mappings
{
    public class VhoWorkHoursMap : IEntityTypeConfiguration<VhoWorkHours>
    {
        public void Configure(EntityTypeBuilder<VhoWorkHours> builder)
        {
            builder.ToTable("VhoWorkHours");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.JusticeUser);
            builder.HasOne(x => x.DayOfWeek);

            builder.Property(x => x.CreatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
            builder.Property(x => x.UpdatedDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
        }
    }
}
