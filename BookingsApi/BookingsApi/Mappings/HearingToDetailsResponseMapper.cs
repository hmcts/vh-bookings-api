using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using BookingsApi.Extensions;
using BookingsApi.Helpers;

namespace BookingsApi.Mappings
{
    public static class HearingToDetailsResponseMapper
    {
        public static HearingDetailsResponse Map(Hearing videoHearing)
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
            
            var allocatedVho = VideoHearingHelper.AllocatedVho(videoHearing);

            var response = new HearingDetailsResponse
            {
                Id = videoHearing.Id,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType?.Name,
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
                HearingTypeCode = videoHearing.HearingType?.Code,
                AllocatedTo = allocatedVho
            };

            return response;
        }
    }
}