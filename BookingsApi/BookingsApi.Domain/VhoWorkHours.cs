using System;

namespace BookingsApi.Domain
{
    public class VhoWorkHours : TrackableEntity<long>
    {
        public Guid JusticeUserId { get; set; }
        public JusticeUser JusticeUser { get; set; }
        public int DayOfWeekId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
