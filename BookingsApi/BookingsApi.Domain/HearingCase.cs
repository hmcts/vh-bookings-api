using System;

namespace BookingsApi.Domain
{
    public class HearingCase
    {
        public long Id { get; set; }
        public long CaseId { get; set; }
        public virtual Case Case { get; set; }
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; set; }
    }
}