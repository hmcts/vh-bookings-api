using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class HearingRequest
    {
        /// <summary>
        /// The id of the hearing
        /// </summary>
        public Guid HearingId { get; set; }
        
        /// <summary>
        /// Scheduled date time of the hearing
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>
        /// Duration of the hearing
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        /// The name of the hearing venue
        /// </summary>
        public string HearingVenueName { get; set; }

        /// <summary>
        /// The hearing room name at the hearing venue
        /// </summary>
        public string HearingRoomName { get; set; }

        /// <summary>
        /// Any other information about the hearing
        /// </summary>
        public string OtherInformation { get; set; }

        /// <summary>
        /// The case number
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        /// Gets or sets the audio recording required flag, value true  is indicated that recording is required, otherwise false
        /// </summary>
        public bool AudioRecordingRequired { get; set; }
        
        /// <summary>
        /// Participants for the hearing
        /// </summary>
        public UpdateHearingParticipantsRequest Participants { get; set; }
        
        /// <summary>
        /// Endpoints for the hearing
        /// </summary>
        public UpdateHearingEndpointsRequest Endpoints { get; set; }
    }
}
