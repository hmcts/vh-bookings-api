using System;
using System.Collections.Generic;
using BookingsApi.Contract.V1.Enums;

namespace BookingsApi.Contract.V1.Responses
{
    /// <summary>
    /// Detailed information of a hearing
    /// </summary>
    public class HearingDetailsResponse
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
        public List<CaseResponse> Cases { get; set; }

        /// <summary>
        ///     List of participants in hearing
        /// </summary>
        public List<ParticipantResponse> Participants { get; set; }
        
        /// <summary>
        ///     List of telephone participants in the hearing
        /// </summary>
        public List<TelephoneParticipantResponse> TelephoneParticipants { get; set; }

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
        public BookingStatus Status { get; set; }

        /// <summary>
        /// QuestionnaireNotRequired
        /// </summary>
        [Obsolete("This property is no longer used.")]
        public bool QuestionnaireNotRequired { get; set; }

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
        public List<EndpointResponse> Endpoints { get; set; }

        /// <summary>
        /// The group id for a hearing
        /// </summary>
        public Guid? GroupId { get; set; }
    }
}