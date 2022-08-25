using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class Allocation : TrackableEntity<long>
    {
        public Guid HearingId { get; set; }
        public Hearing Hearing { get; set; }
        public Guid JusticeUserId { get; set; }
        public JusticeUser JusticeUser { get; set; }
        public int EffortSpent { get; set; }
        public string CreatedBy { get; set; }
    }
}
