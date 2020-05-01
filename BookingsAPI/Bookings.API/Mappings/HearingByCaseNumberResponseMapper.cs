using Bookings.Api.Contract.Responses;
using Bookings.Domain;
using System;
using System.Linq;

namespace Bookings.API.Mappings
{
    public class HearingByCaseNumberResponseMapper
    {
        public HearingByCaseNumberResponse MapHearingToDetailedResponse(Hearing videoHearing, string caseNumber)
        {
            if (videoHearing == null) return new HearingByCaseNumberResponse();

            var @case = videoHearing.GetCases().FirstOrDefault(c => c.Number.ToLower() == caseNumber.ToLower());
            if (@case == null) throw new ArgumentException("Hearing is missing case");

            var judgeParticipant = videoHearing.GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.IsJudge);
            var judgeName = judgeParticipant != null ? judgeParticipant.DisplayName : "";

            var response = new HearingByCaseNumberResponse
            {
                Id = videoHearing.Id,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingVenueName = videoHearing.HearingVenueName,
                CaseName = @case.Name,
                CaseNumber = @case.Number,
                CourtroomAccount = judgeName
            };

            return response;
        }
    }
}
