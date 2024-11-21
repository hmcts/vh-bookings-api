namespace BookingsApi.DAL.Mappings.RefData
{
    public class CaseTypeMap : IEntityTypeConfiguration<CaseType>
    {
        public void Configure(EntityTypeBuilder<CaseType> builder)
        {
            builder.ToTable(nameof(CaseType));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
            builder.Property(x => x.ExpirationDate).HasConversion(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
            
            builder.Ignore(x => x.CaseRoles);
            builder.Ignore(x => x.HearingTypes);
        }
    }
}