﻿using Bookings.Api.Contract.Requests;
using System;
using System.Collections.Generic;

namespace Bookings.AcceptanceTests.Models
{
    internal static class UpdateHearingRequest
    {
        public static Api.Contract.Requests.UpdateHearingRequest BuildRequest(string caseName)
        {
            var caseList = new List<CaseRequest>
            {
                new CaseRequest {Name = caseName, Number = "CaseNumber"}
            };

            return new Api.Contract.Requests.UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester Civil and Family Justice Centre",
                HearingRoomName = "HearingRoomName12345",
                OtherInformation = "OtherInformation12345",
                UpdatedBy = "admin@madeupemail.com",
                Cases = caseList,
                QuestionnaireNotRequired = true,
                AudioRecordingRequired = true
            };
        }
    }
}
