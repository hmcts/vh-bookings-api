namespace BookingsApi.DAL.Mappings
{
    public class AllocationMap : IEntityTypeConfiguration<Allocation>
    {
        public void Configure(EntityTypeBuilder<Allocation> builder)
        {
            builder.ToTable("Allocation");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.JusticeUser);
        }
    }
}
