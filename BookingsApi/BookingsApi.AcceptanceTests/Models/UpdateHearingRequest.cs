using BookingsApi.Contract.Requests;
using System;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;

namespace BookingsApi.AcceptanceTests.Models
{
    internal static class UpdateHearingRequest
    {
        public static BookingsApi.Contract.Requests.UpdateHearingRequest BuildRequest(string caseName)
        {
            var caseList = new List<CaseRequest>
            {
                new CaseRequest {Name = caseName, Number = "CaseNumber"}
            };

            return new BookingsApi.Contract.Requests.UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester Civil and Family Justice Centre",
                HearingRoomName = "HearingRoomName12345",
                OtherInformation = "OtherInformation12345",
                UpdatedBy = "admin@hmcts.net",
                Cases = caseList,
                QuestionnaireNotRequired = false,
                AudioRecordingRequired = true
            };
        }
    }
}
