using System;

namespace BookingsApi.Contract.V2.Requests
{
    public class HearingRequestV2
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
        /// The code of the hearing venue
        /// </summary>
        public string HearingVenueCode { get; set; }

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
        public UpdateHearingParticipantsRequestV2 Participants { get; set; }
        
        /// <summary>
        /// Endpoints for the hearing
        /// </summary>
        public UpdateHearingEndpointsRequestV2 Endpoints { get; set; }
        
        /// <summary>
        /// Judiciary participants for the hearing
        /// </summary>
        public UpdateJudiciaryParticipantsRequestV2 JudiciaryParticipants { get; set; }
    }
}
