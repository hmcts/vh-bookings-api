using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class ChecklistQuestionMap : IEntityTypeConfiguration<ChecklistQuestion>
    {
        public void Configure(EntityTypeBuilder<ChecklistQuestion> builder)
        {
            builder.ToTable("ChecklistQuestion");

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.Role, x.Key}).IsUnique();
            builder.Property(x => x.Role).HasColumnName("RoleId");
            builder.Property(x => x.Key).IsRequired();
        }
    }
}