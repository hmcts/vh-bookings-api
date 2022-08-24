using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class VhoNonAvailabilityMap : IEntityTypeConfiguration<VhoNonAvailability>
    {
        public void Configure(EntityTypeBuilder<VhoNonAvailability> builder)
        {
            builder.ToTable("VhoNonAvailability");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.JusticeUser);
        }
    }
}
