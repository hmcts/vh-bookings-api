using System;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class VhoNonAvailability : TrackableEntity<long>
    {
        public Guid JusticeUserId { get; set; }
        public JusticeUser JusticeUser { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
