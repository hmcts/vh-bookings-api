using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain
{
    public class VideoHearing : Hearing
    {
        protected VideoHearing() { }

        /// <summary>
        /// Instantiate a hearing when the hearing type is known, typically used for V1
        /// </summary>
        public VideoHearing(CaseType caseType,
            HearingType hearingType,
            DateTime scheduledDateTime,
            int scheduledDuration,
            HearingVenue hearingVenue,
            string hearingRoomName,
            string otherInformation,
            string createdBy,
            bool audioRecordingRequired,
            string cancelReason)
            : base(caseType, hearingType, scheduledDateTime, scheduledDuration, hearingVenue, hearingRoomName,
                otherInformation, createdBy, audioRecordingRequired, cancelReason)
        {
        }

        /// <summary>
        /// Instantiate a hearing without a video hearing type, typically used for V2
        /// </summary>
        public VideoHearing(CaseType caseType,
            DateTime scheduledDateTime,
            int scheduledDuration,
            HearingVenue hearingVenue,
            string hearingRoomName,
            string otherInformation,
            string createdBy,
            bool audioRecordingRequired,
            string cancelReason)
            : base(caseType, scheduledDateTime, scheduledDuration, hearingVenue, hearingRoomName, otherInformation,
                createdBy, audioRecordingRequired, cancelReason)
        {
        }

        public override HearingMediumType HearingMediumType { get; protected set; } = HearingMediumType.FullyVideo;
    }
}