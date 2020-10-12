﻿using Bookings.Api.Contract.Responses;
using Bookings.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;

namespace Bookings.API.Mappings
{
    public class AudioRecordedHearingsBySearchResponseMapper
    {
        public List<AudioRecordedHearingsBySearchResponse> MapHearingToDetailedResponse(IEnumerable<Hearing> videoHearing,
            string caseNumber)
        {
            if (videoHearing == null || !videoHearing.Any()) return new List<AudioRecordedHearingsBySearchResponse>();

            var response = new List<AudioRecordedHearingsBySearchResponse>();
            foreach (var hearing in videoHearing)
            {
                var judgeParticipant = hearing.GetParticipants()
                    .FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.IsJudge);
                var courtroomAccountName = judgeParticipant != null ? judgeParticipant.DisplayName : string.Empty;
                var courtroomAccount =
                    (judgeParticipant?.Person != null) ? judgeParticipant.Person.Username : string.Empty;

                var @case = caseNumber.IsNullOrEmpty()
                    ? hearing.GetCases().First()
                    : hearing.GetCases()
                        .FirstOrDefault(c => c.Number.ToLower().Trim().Contains(caseNumber.ToLower().Trim()));

                if (@case == null) throw new ArgumentException("Hearing is missing case");

                var hearingByCaseNumber = new AudioRecordedHearingsBySearchResponse()
                {
                    Id = hearing.Id,
                    ScheduledDateTime = hearing.ScheduledDateTime,
                    HearingVenueName = hearing.HearingVenueName,
                    HearingRoomName = hearing.HearingRoomName,
                    CourtroomAccount = courtroomAccount,
                    CourtroomAccountName = courtroomAccountName,
                    CaseName = @case.Name,
                    CaseNumber = @case.Number,
                    GroupId = hearing.SourceId
                };
                response.Add(hearingByCaseNumber);
            }

            return response;
        }
    }
}
