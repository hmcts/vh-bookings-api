using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings.RefData
{
    public class HearingTypeMap : IEntityTypeConfiguration<HearingType>
    {
        public void Configure(EntityTypeBuilder<HearingType> builder)
        {
            builder.ToTable(nameof(HearingType));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
        }
    }
}