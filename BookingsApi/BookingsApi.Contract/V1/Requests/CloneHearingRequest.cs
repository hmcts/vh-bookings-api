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
        
        /// <summary>
        /// List of dates to create a new hearing on
        /// </summary>
        public IList<DateTime> Dates { get; set; }
        
        /// <summary>
        /// Scheduled duration of the hearing in minutes, defaults to 480
        /// </summary>
        public int ScheduledDuration { get; set; } = 480;
    }
}