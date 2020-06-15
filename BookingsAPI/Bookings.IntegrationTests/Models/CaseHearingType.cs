using System.Collections.Generic;

namespace Bookings.IntegrationTests.Models
{
    public class CaseHearingType
    {
        public string CaseTypeName { get; set; }
        public IEnumerable<string> HearingTypeName { get; set; }
    }
}