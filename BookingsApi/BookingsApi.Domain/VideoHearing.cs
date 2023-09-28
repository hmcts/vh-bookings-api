using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain
{
    public class VideoHearing : Hearing
    {
        protected VideoHearing() { }
        
        public VideoHearing(CaseType caseType,
            DateTime scheduledDateTime,
            int scheduledDuration, 
            HearingVenue hearingVenue,
            string hearingRoomName,
            string otherInformation,
            string createdBy,
            bool audioRecordingRequired, 
            string cancelReason) 
            : base(caseType, scheduledDateTime, scheduledDuration, hearingVenue, hearingRoomName, otherInformation, createdBy, audioRecordingRequired, cancelReason)
        {
        }

        public override HearingMediumType HearingMediumType { get; protected set; } = HearingMediumType.FullyVideo;
    }
}