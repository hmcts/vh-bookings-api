using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2
{
    public static class HearingToDetailsResponseV2Mapper
    {
        public static HearingDetailsResponseV2 Map(Hearing videoHearing)
        {
            var caseMapper = new CaseToResponseV2Mapper();
            var participantMapper = new ParticipantToResponseV2Mapper();
            var judiciaryParticipantMapper = new JudiciaryParticipantToResponseMapper();
            
            var cases = videoHearing.GetCases()
                .Select(x => caseMapper.MapCaseToResponse(x))
                .ToList();

            var participants = videoHearing.GetParticipants()
                .Select(x => participantMapper.MapParticipantToResponse(x))
                .ToList();

            var endpoints = videoHearing.GetEndpoints()
                .Select(EndpointToResponseV2Mapper.MapEndpointToResponse)
                .ToList();

            var judiciaryParticipants = videoHearing.GetJudiciaryParticipants()
                .Select(x => judiciaryParticipantMapper.MapJudiciaryParticipantToResponse(x))
                .ToList();
            
            var response = new HearingDetailsResponseV2
            {
                Id = videoHearing.Id,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                ServiceId = videoHearing.CaseType.ServiceId,
                ServiceName = videoHearing.CaseType.Name,
                HearingVenueCode = videoHearing.HearingVenue.VenueCode,
                HearingVenueName = videoHearing.HearingVenue.Name,
                IsHearingVenueScottish = videoHearing.HearingVenue.IsScottish,
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
    }
}