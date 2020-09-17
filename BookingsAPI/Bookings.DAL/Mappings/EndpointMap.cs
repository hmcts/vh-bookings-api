using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class EndpointMap : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.ToTable("Endpoint");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.DisplayName).IsRequired();
            builder.HasIndex(x => x.Sip).IsUnique();
            builder.Property(x => x.Pin).IsRequired();
            builder.HasOne(x => x.DefenceAdvocate);
        }
    }
}