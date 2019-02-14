using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class AddressMap : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.HouseNumber);
            builder.Property(x => x.Street);
            builder.Property(x => x.Postcode);
        }
    }
}