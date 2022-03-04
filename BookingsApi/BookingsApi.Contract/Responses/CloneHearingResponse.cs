using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class CloneHearingResponse
    {
        public List<Guid> NewHearingIds { get; set; }
    }
}
