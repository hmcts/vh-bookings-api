using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    /// <summary>
    /// Request to clone a hearing
    /// </summary>
    public class CloneHearingRequestV2
    {
        /// <summary>
        /// List of dates to create a new hearing on
        /// </summary>
        public IList<DateTime> Dates { get; set; } = new List<DateTime>();

        /// <summary>
        /// Scheduled duration of the hearing in minutes, defaults to 480
        /// </summary>
        public int ScheduledDuration { get; set; } = Constants.CloneHearings.DefaultScheduledDuration;
    }
}