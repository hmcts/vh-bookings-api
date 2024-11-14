namespace BookingsApi.DAL.Mappings.RefData
{
    /// <summary>
    /// TODO: remove as part of https://tools.hmcts.net/jira/browse/VIH-10899
    /// </summary>
    public class CaseRoleMap : IEntityTypeConfiguration<CaseRole>
    {
        public void Configure(EntityTypeBuilder<CaseRole> builder)
        {
            builder.ToTable(nameof(CaseRole));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
            builder.Property(x => x.Group);

            builder.HasMany(x => x.HearingRoles).WithOne();
        }
    }
}