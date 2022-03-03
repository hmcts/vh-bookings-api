using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain.RefData
{
    public class HearingType : Entity<int>
    {
        public HearingType(string name)
        {
            Name = name;
            Live = true; // Default to true
        }
        public string Name { get; set; }
        public bool Live { get; set; }
    }
}