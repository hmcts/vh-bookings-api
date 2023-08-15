namespace BookingsApi.DAL.Mappings
{
    public class DayOfWeekrMap : IEntityTypeConfiguration<Domain.DayOfWeek>
    {
        public void Configure(EntityTypeBuilder<Domain.DayOfWeek> builder)
        {
            builder.ToTable("DayOfWeek");

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Day).IsUnique();
        }
    }
}
