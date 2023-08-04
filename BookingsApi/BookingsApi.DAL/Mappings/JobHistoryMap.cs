namespace BookingsApi.DAL.Mappings
{
    public class JobHistoryMap : IEntityTypeConfiguration<JobHistory>
    {
        public void Configure(EntityTypeBuilder<JobHistory> builder)
        {
            builder.ToTable("JobHistory");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.LastRunDate);
            builder.Property(x => x.LastRunDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
        }
    }
}
