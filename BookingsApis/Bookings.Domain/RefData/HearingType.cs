using Bookings.Domain.Ddd;

namespace Bookings.Domain.RefData
{
    public class HearingType : Entity<int>
    {
        public HearingType(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}