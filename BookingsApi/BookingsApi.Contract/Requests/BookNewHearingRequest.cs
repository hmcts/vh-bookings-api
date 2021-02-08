using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class BookNewHearingRequest
    {
        public BookNewHearingRequest()
        {
            Cases = new List<CaseRequest>();
            Participants = new List<ParticipantRequest>();
            LinkedParticipants = new List<LinkedParticipantRequest>();
            Endpoints = new List<EndpointRequest>();
        }

        /// <summary>
        ///     The date and time for a hearing
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>
        ///     The duration of a hearing (number of minutes)
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        ///     The name of the hearing venue
        /// </summary>
        public string HearingVenueName { get; set; }

        /// <summary>
        ///     The name of the case type
        /// </summary>
        public string CaseTypeName { get; set; }

        /// <summary>
        ///     The name of the hearing type
        /// </summary>
        public string HearingTypeName { get; set; }

        /// <summary>
        ///     List of cases associated to the hearing
        /// </summary>
        public List<CaseRequest> Cases { get; set; }

        /// <summary>
        ///     List of participants in hearing
        /// </summary>
        public List<ParticipantRequest> Participants { get; set; }

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
        /// QuestionnaireNotRequired
        /// </summary>
        public bool QuestionnaireNotRequired { get; set; }

        /// <summary>
        /// Gets or sets the audio recording required flag, value true  is indicated that recording is required, otherwise false
        /// </summary>
        public bool AudioRecordingRequired { get; set; }

        public List<EndpointRequest> Endpoints { get; set; }
        
        public List<LinkedParticipantRequest> LinkedParticipants { get; set; }
    }
}