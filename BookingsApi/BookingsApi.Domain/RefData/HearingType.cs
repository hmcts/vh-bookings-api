using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain.RefData
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