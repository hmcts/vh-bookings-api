using System;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class Allocation : TrackableEntity<long>
    {
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; set; }
        public Guid JusticeUserId { get; set; }
        public virtual JusticeUser JusticeUser { get; set; }
        public int EffortSpent { get; set; }
        public string CreatedBy { get; set; }
    }
}
