using System.Linq;
using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public class HearingToDetailResponseMap
    {
        public HearingDetailsResponse MapDomainToResponse(Hearing videoHearing)
        {
            var caseMapper = new CaseToResponseMap();
            var participantMapper = new ParticipantToResponseMap();
            
            var cases = videoHearing.GetCases()
                .Select(x => caseMapper.MapDomainToResponse(x))
                .ToList();

            var participants = videoHearing.GetParticipants()
                .Select(x => participantMapper.MapDomainToResponse(x))
                .ToList();

            var response = new HearingDetailsResponse
            {
                Id = videoHearing.Id,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType.Name,
                CaseTypeName = videoHearing.CaseType.Name,
                HearingVenueName = videoHearing.HearingVenueName,
                Cases = cases,
                Participants = participants
            };
            return response;
        }
    }
}