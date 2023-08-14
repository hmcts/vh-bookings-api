namespace BookingsApi.DAL.Mappings
{
    public class JusticeUserMap : IEntityTypeConfiguration<JusticeUser>
    {
        public void Configure(EntityTypeBuilder<JusticeUser> builder)
        {
            builder.ToTable("JusticeUser");
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.VhoWorkHours).WithOne(x => x.JusticeUser).HasForeignKey(x => x.JusticeUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.VhoNonAvailability).WithOne(x => x.JusticeUser).HasForeignKey(x => x.JusticeUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Allocations).WithOne(x => x.JusticeUser).HasForeignKey(x => x.JusticeUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.Username).HasMaxLength(450);
            builder.Property(u => u.UserRoleId).IsRequired(false).HasDefaultValue(null);
            builder.HasMany(u => u.JusticeUserRoles).WithOne(x => x.JusticeUser).HasForeignKey(x => x.JusticeUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class JusticeUserRoleMap : IEntityTypeConfiguration<JusticeUserRole>
    {
        public void Configure(EntityTypeBuilder<JusticeUserRole> builder)
        {
            builder.ToTable("JusticeUserRoles");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.JusticeUser).WithMany(x => x.JusticeUserRoles).HasForeignKey(x => x.JusticeUserId);
            builder.HasOne(x => x.UserRole).WithMany(x => x.JusticeUserRoles).HasForeignKey(x => x.UserRoleId);
        }
    }
}
