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
        }
    }
}
