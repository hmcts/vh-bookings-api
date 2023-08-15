namespace BookingsApi.DAL.Mappings.RefData
{
    public class UserRoleMap : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable(nameof(UserRole));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
        }
    }
}