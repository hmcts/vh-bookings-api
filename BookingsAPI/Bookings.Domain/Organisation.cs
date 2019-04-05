using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class Organisation : Entity<long>
    {
        public Organisation() { }

        public Organisation(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}