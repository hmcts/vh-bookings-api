using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings.RefData
{
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