using System;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;

namespace Bookings.Domain
{
    public class VideoHearing : Hearing
    {
        protected VideoHearing() { }
        public VideoHearing(CaseType caseType,HearingType hearingType, DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue, string hearingRoomName, string otherInformation, string createdBy) : 
            base(caseType, hearingType, scheduledDateTime, scheduledDuration, hearingVenue, hearingRoomName, otherInformation, createdBy)
        {
        }

        public override HearingMediumType HearingMediumType { get; protected set; } = HearingMediumType.FullyVideo;
    }
}