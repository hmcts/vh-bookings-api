using System;

namespace BookingsApi.Domain
{
    public class VhoNonAvailability : TrackableEntity<long>
    {
        public Guid JusticeUserId { get; set; }
        public JusticeUser JusticeUser { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string CreatedBy { get; set; }

        public VhoNonAvailability()
        {
        }
        
        public VhoNonAvailability(long id)
        {
            Id = id;
        }
        public bool Deleted { get; set; }
        
        public void Update(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public void Delete()
        {
            Deleted = true;
        }
    }
}
