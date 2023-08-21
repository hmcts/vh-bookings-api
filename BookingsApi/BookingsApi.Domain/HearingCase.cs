using System;

namespace BookingsApi.Domain
{
    public class HearingCase : TrackableEntity<long>
    {
        public long CaseId { get; set; }
        public virtual Case Case { get; set; }
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; set; }
    }
}