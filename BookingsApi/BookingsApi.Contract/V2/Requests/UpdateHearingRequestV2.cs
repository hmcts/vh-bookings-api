using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateHearingRequestV2
    {
        /// <summary>
        ///     Hearing Schedule Date and Time (if changed)
        /// </summary>
        public DateTime? ScheduledDateTime { get; set; }

        /// <summary>
        ///     Duration of the hearing
        /// </summary>
        public int ScheduledDuration { get; set; }
        
        /// <summary>
        ///     The code of the hearing venue
        /// </summary>
        public string HearingVenueCode { get; set; }

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
        public List<CaseRequestV2> Cases { get; set; }

        /// <summary>
        ///     User updated the hearing record
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the audio recording required flag, value true  is indicated that recording is required, otherwise false
        /// </summary>
        public bool? AudioRecordingRequired { get; set; }
    }
}