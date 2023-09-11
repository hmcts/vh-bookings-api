using System;
using System.Collections.Generic;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Contract.V2.Enums;
using Newtonsoft.Json;

namespace BookingsApi.Contract.V2.Responses;

public class HearingDetailsResponseV2
{
    /// <summary>
        ///     Hearing Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The date and time for a hearing
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>
        ///     The duration of a hearing (number of minutes)
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        ///     The code for the hearing venue
        /// </summary>
        public string HearingVenueCode { get; set; }
        
        /// <summary>
        ///     The name for the hearing venue
        /// </summary>
        public string HearingVenueName { get; set; }
        
        /// <summary>
        ///     Is the hearing venue Scottish
        /// </summary>
        public bool IsHearingVenueScottish { get; set; }

        /// <summary>
        ///     The ID for the service
        /// </summary>
        public string ServiceId { get; set; }
        
        /// <summary>
        ///     The name for the service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        ///     The code for the hearing type
        /// </summary>
        public string HearingTypeCode { get; set; }

        /// <summary>
        ///     List of cases associated to the hearing
        /// </summary>
        public List<CaseResponseV2> Cases { get; set; }

        /// <summary>
        ///     List of participants in hearing
        /// </summary>
        public List<ParticipantResponseV2> Participants { get; set; }

        /// <summary>
        ///     The hearing room name at the hearing venue
        /// </summary>
        public string HearingRoomName { get; set; }

        /// <summary>
        ///     Any other information about the hearing
        /// </summary>
        public string OtherInformation { get; set; }

        /// <summary>
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     The VH admin username that created the hearing
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        ///     The last date and time any details for the hearing was updated
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        ///     User id of the who last updated the hearing
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        ///     The VH admin username that confirmed the hearing
        /// </summary>
        public string ConfirmedBy { get; set; }

        /// <summary>
        ///     The date and time when the hearing was confirmed
        /// </summary>
        public DateTime? ConfirmedDate { get; set; }

        /// <summary>
        /// Gets or sets the booking status of the hearing
        /// </summary>
        public BookingStatusV2 Status { get; set; }
        
        /// <summary>
        /// Gets or sets the audio recording required flag, value true  is indicated that recording is required, otherwise false
        /// </summary>
        public bool AudioRecordingRequired { get; set; }

        /// <summary>
        /// Gets or sets the hearing cancel reason
        /// </summary>
        public string CancelReason { get; set; }

        /// <summary>
        /// Gets the endpoints for a hearing
        /// </summary>
        public List<EndpointResponseV2> Endpoints { get; set; }

        /// <summary>
        /// The group id for a hearing
        /// </summary>
        public Guid? GroupId { get; set; }
        
        /// <summary>
        /// List of judiciary participants in a hearing
        /// </summary>
        [JsonProperty("judicial_office_holders")]
        public List<JudiciaryParticipantResponse> JudiciaryParticipants { get; set; }
}