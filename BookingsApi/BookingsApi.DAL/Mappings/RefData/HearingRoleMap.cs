namespace BookingsApi.DAL.Mappings.RefData
{
    public class HearingRoleMap : IEntityTypeConfiguration<HearingRole>
    {
        public void Configure(EntityTypeBuilder<HearingRole> builder)
        {
            builder.ToTable(nameof(HearingRole));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
            builder.Property(x => x.UserRoleId);
            builder.Property(x => x.CaseRoleId).IsRequired(false);

            builder.HasOne(x => x.UserRole);
            builder.Property(x => x.Live).HasDefaultValue(true);
            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}