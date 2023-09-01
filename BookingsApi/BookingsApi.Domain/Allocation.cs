using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class Allocation : TrackableEntity<long>
    {
        public Allocation()
        {
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; set; }
        public Guid JusticeUserId { get; set; }
        public virtual JusticeUser JusticeUser { get; set; }
        public int EffortSpent { get; set; }
        public string CreatedBy { get; set; }
    }
}
