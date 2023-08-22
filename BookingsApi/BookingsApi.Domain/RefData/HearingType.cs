
using System;

namespace BookingsApi.Domain.RefData
{
    public class HearingType : TrackableEntity<int>
    {
        public HearingType(string name)
        {
            Name = name;
            Live = true; // Default to true
        }
        public string Name { get; set; }
        public bool Live { get; set; }
        public string Code { get; set; }
        public string WelshName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        
        public bool HasExpired()
        {
            return ExpirationDate != null && ExpirationDate <= DateTime.UtcNow;
        }
    }
}