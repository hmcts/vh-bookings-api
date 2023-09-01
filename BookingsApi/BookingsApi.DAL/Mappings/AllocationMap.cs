namespace BookingsApi.DAL.Mappings
{
    public class AllocationMap : IEntityTypeConfiguration<Allocation>
    {
        public void Configure(EntityTypeBuilder<Allocation> builder)
        {
            builder.ToTable("Allocation");

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.JusticeUserId, x.HearingId}).IsUnique();
            builder.HasOne(x => x.JusticeUser).WithMany(x=>x.Allocations).HasForeignKey(x => x.JusticeUserId);
            builder.HasOne(x => x.Hearing).WithMany(x=>x.Allocations).HasForeignKey(x => x.HearingId);
        }
    }
}
