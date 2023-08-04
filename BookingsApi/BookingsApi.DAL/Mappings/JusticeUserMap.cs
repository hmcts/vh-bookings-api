namespace BookingsApi.DAL.Mappings
{
    public class JusticeUserMap : IEntityTypeConfiguration<JusticeUser>
    {
        public void Configure(EntityTypeBuilder<JusticeUser> builder)
        {
            builder.ToTable("JusticeUser");
            builder.HasKey(x => x.Id);
            builder.HasMany<VhoWorkHours>("VhoWorkHours").WithOne("JusticeUser").HasForeignKey(x => x.JusticeUserId);
            builder.HasMany<VhoNonAvailability>("VhoNonAvailability").WithOne("JusticeUser").HasForeignKey(x => x.JusticeUserId);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.Username).HasMaxLength(450);
            builder.Property(u => u.UserRoleId).IsRequired(false).HasDefaultValue(null);
        }
    }
}
