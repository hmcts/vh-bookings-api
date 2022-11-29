using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain;
using BookingsApi.Extensions;

namespace BookingsApi.Mappings
{
    public class HearingToDetailsResponseMapper
    {
        public HearingDetailsResponse MapHearingToDetailedResponse(Hearing videoHearing)
        {
            var caseMapper = new CaseToResponseMapper();
            var participantMapper = new ParticipantToResponseMapper();
            
            var cases = videoHearing.GetCases()
                .Select(x => caseMapper.MapCaseToResponse(x))
                .ToList();

            var participants = videoHearing.GetParticipants()
                .Select(x => participantMapper.MapParticipantToResponse(x))
                .ToList();

            var endpoints = videoHearing.GetEndpoints()
                .Select(EndpointToResponseMapper.MapEndpointToResponse)
                .ToList();
            
            var allocatedVho = "Not Allocated";
            var isScottishVenue =
                HearingScottishVenueNames.ScottishHearingVenuesList.Any(venueName =>
                    venueName == videoHearing.HearingVenueName);
            if (videoHearing.AllocatedTo == null) {
                if (isScottishVenue || videoHearing.CaseTypeId == 3) // not required if scottish venue or generic type
                {
                    allocatedVho = "Not Required";
                }
            } else {
                allocatedVho = videoHearing.AllocatedTo.ContactEmail;
            }
            
            var response = new HearingDetailsResponse
            {
                Id = videoHearing.Id,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType.Name,
                CaseTypeName = videoHearing.CaseType.Name,
                HearingVenueName = videoHearing.HearingVenueName,
                Cases = cases,
                Participants = participants,
                HearingRoomName = videoHearing.HearingRoomName,
                OtherInformation = videoHearing.OtherInformation,
                CreatedBy = videoHearing.CreatedBy,
                CreatedDate = videoHearing.CreatedDate,
                UpdatedBy = videoHearing.UpdatedBy,
                UpdatedDate = videoHearing.UpdatedDate,
                ConfirmedBy = videoHearing.ConfirmedBy,
                ConfirmedDate = videoHearing.ConfirmedDate,
                Status = videoHearing.Status.MapToContractEnum(),
                QuestionnaireNotRequired = videoHearing.QuestionnaireNotRequired,
                AudioRecordingRequired = videoHearing.AudioRecordingRequired,
                CancelReason = videoHearing.CancelReason,
                GroupId = videoHearing.SourceId,
                Endpoints = endpoints,
                HearingTypeCode = videoHearing.HearingType.Code,
                AllocatedVho = allocatedVho
            };

            return response;
        }
    }
}