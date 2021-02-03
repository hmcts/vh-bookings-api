using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class CaseMap : IEntityTypeConfiguration<Case>
    {
        public void Configure(EntityTypeBuilder<Case> builder)
        {
            builder.ToTable("Case");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
            builder.Property(x => x.Number);
            builder.Property(x => x.IsLeadCase);
            builder.HasMany(x => x.HearingCases).WithOne(x => x.Case).HasForeignKey(x => x.CaseId);
        }
    }
}