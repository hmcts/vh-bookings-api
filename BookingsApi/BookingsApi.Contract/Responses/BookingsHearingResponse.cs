using System;
using BookingsApi.Contract.Enums;

namespace BookingsApi.Contract.Responses
{
    public class BookingsHearingResponse
    {
        /// <summary>
        /// Gets or sets the hearing ID.
        /// </summary>
        public Guid HearingId { get; set; }

        /// <summary>
        /// Gets or sets the hearing number/reference.
        /// </summary>
        public string HearingNumber { get; set; }

        /// <summary>
        /// Gets or sets the hearing title/name.
        /// </summary>
        public string HearingName { get; set; }

        /// <summary>
        /// Gets or sets the hearing scheduled date and time.
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>
        /// Gets or sets the hearing duration.
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        /// Gets or sets the name of the case type.
        /// </summary>
        public string CaseTypeName { get; set; }

        /// <summary>
        /// Gets or sets the hearing case type.
        /// </summary>
        public string HearingTypeName { get; set; }

        /// <summary>
        /// Gets or sets the court room.
        /// </summary>
        public string CourtRoom { get; set; }

        /// <summary>
        /// Gets or sets the court address.
        /// </summary>
        public string CourtAddress { get; set; }

        /// <summary>
        /// Gets or sets Judge name.
        /// </summary>
        public string JudgeName { get; set; }

        /// <summary>
        /// Gets or sets the name/email person who create the hearing.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date of hearing.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the name/email person who last edit the hearing.
        /// </summary>
        public string LastEditBy { get; set; }

        /// <summary>
        /// Gets or sets the last edited date of hearing.
        /// </summary>
        public DateTime? LastEditDate { get; set; }
        /// <summary>
        ///     The VH admin username that confirmed the hearing
        /// </summary>
        public string ConfirmedBy { get; set; }

        /// <summary>
        ///     The date and time when the hearing was confirmed
        /// </summary>
        public DateTime? ConfirmedDate { get; set; }

        /// <summary>
        /// Gets the scheduled date without time.
        /// </summary>
        public DateTime HearingDate
        {
            get
            {
                return ScheduledDateTime.Date;
            }
        }

        /// <summary>
        /// Gets or sets the booking status of the hearing.
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// QuestionnaireNotRequired
        /// </summary>
        public bool QuestionnaireNotRequired { get; set; }

        /// <summary>
        /// Gets or sets the audio recording required flag, value true  is indicated that recording is required, otherwise false
        /// </summary>
        public bool AudioRecordingRequired { get; set; }

        public string CancelReason { get; set; }
        
        /// <summary>
        /// The group id for a hearing
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Gets or sets Judge/Court account (username).
        /// </summary>
        public string CourtRoomAccount { get; set; }
        
        /// <summary>
        /// Gets or sets Allocated VHO.
        /// </summary>
        public string AllocatedTo { get; set; }
    }
}