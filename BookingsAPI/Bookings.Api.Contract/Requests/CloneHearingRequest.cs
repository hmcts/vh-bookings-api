using System;
using System.Collections.Generic;

namespace Bookings.Api.Contract.Requests
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