using Bookings.Api.Contract.Responses;
using Bookings.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.API.Mappings
{
    public class HearingByCaseNumberResponseMapper
    {
        public List<HearingsByCaseNumberResponse> MapHearingToDetailedResponse(IEnumerable<Hearing> videoHearing, string caseNumber)
        {
            if (videoHearing == null || !videoHearing.Any()) return new List<HearingsByCaseNumberResponse>();

            var response = new List<HearingsByCaseNumberResponse>();
            foreach(var hearing in videoHearing)
            {
                var judgeParticipant = hearing.GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.IsJudge);
                var courtroomAccountName = judgeParticipant != null ? judgeParticipant.DisplayName : string.Empty;
                var courtroomAccount = (judgeParticipant != null && judgeParticipant.Person != null) ? judgeParticipant.Person.Username : string.Empty;
                var @case = hearing.GetCases().FirstOrDefault(c => c.Number.ToLower() == caseNumber.ToLower());
                if (@case == null) throw new ArgumentException("Hearing is missing case");

                var hearingByCaseNumber = new HearingsByCaseNumberResponse()
                {
                    Id = hearing.Id,
                    ScheduledDateTime = hearing.ScheduledDateTime,
                    HearingVenueName = hearing.HearingVenueName,
                    HearingRoomName = hearing.HearingRoomName,
                    CourtroomAccount = courtroomAccount,
                    CourtroomAccountName = courtroomAccountName,
                    CaseName = @case.Name,
                    CaseNumber = @case.Number
                };
                response.Add(hearingByCaseNumber);
            }
            return response;
        }
    }
}
