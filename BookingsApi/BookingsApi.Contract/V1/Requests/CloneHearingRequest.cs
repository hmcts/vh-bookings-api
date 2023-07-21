using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class CloneHearingRequest
    {
        public CloneHearingRequest()
        {
            Dates = new List<DateTime>();
        }
        public IList<DateTime> Dates { get; set; }
    }
}