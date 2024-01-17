using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Extensions;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V1.Extensions;

namespace BookingsApi.Mappings.V1
{
    public static class HearingToDetailsResponseMapper
    {
        public static HearingDetailsResponse Map(Hearing videoHearing)
        {
            var caseMapper = new CaseToResponseMapper();
            var participantMapper = new ParticipantToResponseMapper();
            var judiciaryParticipantMapper = new JudiciaryParticipantToResponseMapper();
            
            var cases = videoHearing.GetCases()
                .Select(x => caseMapper.MapCaseToResponse(x))
                .ToList();

            var participants = videoHearing.GetParticipants()
                .Select(x => participantMapper.MapParticipantToResponse(x))
                .ToList();

            var endpoints = videoHearing.GetEndpoints()
                .Select(EndpointToResponseMapper.MapEndpointToResponse)
                .ToList();

            var judiciaryParticipants = videoHearing.GetJudiciaryParticipants()
                .Select(x => judiciaryParticipantMapper.MapJudiciaryParticipantToResponse(x))
                .ToList();
            
            var response = new HearingDetailsResponse
            {
                Id = videoHearing.Id,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType?.Name,
                CaseTypeName = videoHearing.CaseType.Name,
                HearingVenueName = videoHearing.HearingVenue.Name,
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
                AudioRecordingRequired = videoHearing.AudioRecordingRequired,
                CancelReason = videoHearing.CancelReason,
                GroupId = videoHearing.SourceId,
                Endpoints = endpoints,
                JudiciaryParticipants = judiciaryParticipants
            };
            response.TrimAllStringsRecursively();
            return response;
        }

        public static HearingDetailsResponse Map(Hearing videoHearing, List<VideoHearing> hearingsInGroup)
        {
            var response = Map(videoHearing);

            if (hearingsInGroup == null || !hearingsInGroup.Any()) return response;

            response.MultiDayHearingEndTime = hearingsInGroup.ScheduledEndTimeOfLastHearing();

            return response;
        }
    }
}