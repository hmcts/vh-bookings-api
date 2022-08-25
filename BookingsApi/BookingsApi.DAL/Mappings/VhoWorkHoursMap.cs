using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class VhoWorkHoursMap : IEntityTypeConfiguration<VhoWorkHours>
    {
        public void Configure(EntityTypeBuilder<VhoWorkHours> builder)
        {
            builder.ToTable("VhoWorkHours");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.JusticeUser);
            builder.HasOne(x => x.DayOfWeek);
        }
    }
}
