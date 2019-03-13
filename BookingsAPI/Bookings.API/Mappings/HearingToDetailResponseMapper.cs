using System.Linq;
using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public class HearingToDetailResponseMapper
    {
        public HearingDetailsResponse MapHearingToDetailedResponse(Hearing hearing)
        {
            var caseMapper = new CaseToResponseMapper();
            var participantMapper = new ParticipantToResponseMapper();
            
            var cases = hearing.GetCases()
                .Select(x => caseMapper.MapCaseToResponse(x))
                .ToList();

            var participants = hearing.GetParticipants()
                .Select(x => participantMapper.MapParticipantToResponse(x))
                .ToList();

            var response = new HearingDetailsResponse
            {
                Id = hearing.Id,
                ScheduledDuration = hearing.ScheduledDuration,
                ScheduledDateTime = hearing.ScheduledDateTime,
                HearingTypeName = hearing.HearingType.Name,
                CaseTypeName = hearing.CaseType.Name,
                HearingVenueName = hearing.HearingVenueName,
                Cases = cases,
                Participants = participants
            };
            return response;
        }
    }
}