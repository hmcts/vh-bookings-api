using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class JobHistoryMap : IEntityTypeConfiguration<JobHistory>
    {
        public void Configure(EntityTypeBuilder<JobHistory> builder)
        {
            builder.ToTable("JobHistory");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.LastRunDate);
        }
    }
}
