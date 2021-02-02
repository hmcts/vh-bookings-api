using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings.RefData
{
    public class CaseTypeMap : IEntityTypeConfiguration<CaseType>
    {
        public void Configure(EntityTypeBuilder<CaseType> builder)
        {
            builder.ToTable(nameof(CaseType));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);

            builder.HasMany(x => x.HearingTypes);
            builder.HasMany(x => x.CaseRoles);
        }
    }
}