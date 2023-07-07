using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class HearingVenueMap : IEntityTypeConfiguration<HearingVenue>
    {
        public void Configure(EntityTypeBuilder<HearingVenue> builder)
        {
            builder.ToTable(nameof(HearingVenue));
            builder.Property(x => x.Id);
            builder.HasKey(x => x.Name);
            builder.HasIndex(x => x.EpimsCode);
            builder.Property(x => x.IsScottish);
            builder.Property(x => x.IsWorkAllocationEnabled);
        }
    }
}