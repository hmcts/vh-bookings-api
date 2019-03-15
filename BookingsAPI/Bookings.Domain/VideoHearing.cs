using System;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;

namespace Bookings.Domain
{
    public class VideoHearing : Hearing
    {
        protected VideoHearing() { }
        public VideoHearing(CaseType caseType,HearingType hearingType, DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue, string otherInformation, string hearingRoomName) : 
            base(caseType, hearingType, scheduledDateTime, scheduledDuration, hearingVenue, otherInformation, hearingRoomName)
        {
        }

        public override HearingMediumType HearingMediumType { get; protected set; } = HearingMediumType.FullyVideo;
    }
}