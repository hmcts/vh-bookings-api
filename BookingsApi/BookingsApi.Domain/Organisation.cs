using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
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