using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingsApi.DAL.Mappings
{
    public class JudiciaryPersonMap : IEntityTypeConfiguration<JudiciaryPerson>
    {
        public void Configure(EntityTypeBuilder<JudiciaryPerson> builder)
        {
            builder.ToTable(nameof(JudiciaryPerson));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasIndex(x => x.ExternalRefId).IsUnique();
            builder.Property(x => x.ExternalRefId);
            builder.Property(x => x.PersonalCode);
            builder.Property(x => x.Title);
            builder.Property(x => x.KnownAs);
            builder.Property(x => x.Surname);
            builder.Property(x => x.Fullname);
            builder.Property(x => x.PostNominals);
            builder.Property(x => x.Email);

            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
        }
    }
}