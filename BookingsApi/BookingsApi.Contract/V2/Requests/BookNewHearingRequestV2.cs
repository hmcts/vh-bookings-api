using System;
using System.Collections.Generic;
using System.ComponentModel;
using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Contract.V2.Requests
{
    /// <summary>
    /// Book a new hearing request model based on using codes
    /// </summary>
    public class BookNewHearingRequestV2
    {
        /// <summary>
        ///     The date and time for a hearing
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>
        ///     The duration of a hearing (number of minutes)
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        ///     The code of the hearing venue
        /// </summary>
        public string HearingVenueCode { get; set; }

        /// <summary>
        ///     The service Id
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// The code of the hearing type
        /// </summary>
        public string HearingTypeCode { get; set; }

        /// <summary>
        ///     List of cases associated to the hearing
        /// </summary>
        public List<CaseRequestV2> Cases { get; set; } = new();

        /// <summary>
        ///     List of participants in hearing
        /// </summary>
        public List<ParticipantRequestV2> Participants { get; set; } = new();

        /// <summary>
        ///     The hearing room name at the hearing venue
        /// </summary>
        public string HearingRoomName { get; set; }

        /// <summary>
        ///     Any other information about the hearing
        /// </summary>
        public string OtherInformation { get; set; }

        /// <summary>
        ///     The VH admin username that created the hearing
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the audio recording required flag, value true  is indicated that recording is required, otherwise false
        /// </summary>
        [DefaultValue(false)]
        public bool AudioRecordingRequired { get; set; } = false;
        
        /// <summary>
        /// Is the booking part of a multi-day hearing?
        /// </summary>
        [DefaultValue(false)]
        public bool IsMultiDayHearing { get; set; } = false;

        public List<EndpointRequestV2> Endpoints { get; set; } = new();

        public List<LinkedParticipantRequestV2> LinkedParticipants { get; set; } = new();

        public List<JudiciaryParticipantRequest> JudiciaryParticipants { get; set; } = new();
    }
}