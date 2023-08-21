using System;

namespace BookingsApi.Domain
{
    public class VhoWorkHours : TrackableEntity<long>
    {
        public VhoWorkHours()
        {
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
        public Guid JusticeUserId { get; set; }
        public virtual JusticeUser JusticeUser { get; set; }
        public int DayOfWeekId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string CreatedBy { get; set; }
        public System.DayOfWeek SystemDayOfWeek
        {
            get
            {
                if (DayOfWeekId == 7)
                {
                    return System.DayOfWeek.Sunday;
                }

                return (System.DayOfWeek)DayOfWeekId;
            }
        }
        public bool Deleted { get; private set; }

        public void Delete()
        {
            Deleted = true;
        }
    }
}
