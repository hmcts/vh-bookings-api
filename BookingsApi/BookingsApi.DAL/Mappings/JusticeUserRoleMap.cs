namespace BookingsApi.DAL.Mappings;

public class JusticeUserRoleMap : IEntityTypeConfiguration<JusticeUserRole>
{
    public void Configure(EntityTypeBuilder<JusticeUserRole> builder)
    {
        builder.ToTable("JusticeUserRoles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.JusticeUserId).IsRequired();
        builder.Property(x => x.UserRoleId).IsRequired();
        builder.HasOne(x => x.JusticeUser).WithMany(x => x.JusticeUserRoles).HasForeignKey(x => x.JusticeUserId);
        builder.HasOne(x => x.UserRole).WithMany(x => x.JusticeUserRoles).HasForeignKey(x => x.UserRoleId);
    }
}