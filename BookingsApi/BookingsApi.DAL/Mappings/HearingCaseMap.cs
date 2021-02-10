using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class HearingCaseMap : IEntityTypeConfiguration<HearingCase>
    {
        public void Configure(EntityTypeBuilder<HearingCase> builder)
        {
            builder.ToTable("HearingCase");

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.CaseId, x.HearingId}).IsUnique();
            builder.HasOne(x => x.Case).WithMany(x => x.HearingCases).HasForeignKey(x => x.CaseId);
            builder.HasOne(x => x.Hearing).WithMany("HearingCases").HasForeignKey(x => x.HearingId);
        }
    }
}