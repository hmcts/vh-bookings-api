using System;
using System.Collections.Generic;

namespace Bookings.Api.Contract.Requests
{
    public class UpdateHearingRequest
    {
        /// <summary>
        ///     Hearing Schedule Date and Time
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>
        ///     Duration of the hearing
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        /// The name of the hearing venue
        /// </summary>
        public string HearingVenueName { get; set; }

        /// <summary>
        ///     The hearing room name at the hearing venue
        /// </summary>
        public string HearingRoomName { get; set; }

        /// <summary>
        ///     Any other information about the hearing
        /// </summary>
        public string OtherInformation { get; set; }

        /// <summary>
        ///     List of cases associated to the hearing
        /// </summary>
        public List<CaseRequest> Cases { get; set; }

        /// <summary>
        ///     User updated the hearing record
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}