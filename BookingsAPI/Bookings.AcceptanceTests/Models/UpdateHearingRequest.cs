using System;

namespace Bookings.AcceptanceTests.Models
{
    internal static class UpdateHearingRequest
    {
        public static Api.Contract.Requests.UpdateHearingRequest BuildRequest()
        {
            return new Api.Contract.Requests.UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester Civil and Family Justice Centre",
                OtherInformation = "OtherInformation 02",
                HearingRoomName = "Room 02"
            };
        }
    }
}
