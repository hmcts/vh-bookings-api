using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings.RefData
{
    public class HearingRoleMap : IEntityTypeConfiguration<HearingRole>
    {
        public void Configure(EntityTypeBuilder<HearingRole> builder)
        {
            builder.ToTable(nameof(HearingRole));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
            builder.Property(x => x.UserRoleId);

            builder.HasOne(x => x.UserRole);
            builder.Property(x => x.Live).HasDefaultValue(true);
        }
    }
}