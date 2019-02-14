using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class Organisation : Entity<long>
    {
        public string Name { get; set; }
    }
}