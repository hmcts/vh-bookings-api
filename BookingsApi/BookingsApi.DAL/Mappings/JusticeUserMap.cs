using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        }
    }
}
