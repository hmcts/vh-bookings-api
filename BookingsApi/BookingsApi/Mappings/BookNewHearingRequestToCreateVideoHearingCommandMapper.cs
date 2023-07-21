using System.Collections.Generic;
using System.Linq;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Dtos;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Mappings
{
    public static class BookNewHearingRequestToCreateVideoHearingCommandMapper
    {
        public static CreateVideoHearingCommand Map(
            BookNewHearingRequest request,
            CaseType caseType,
            HearingType hearingType,
            HearingVenue venue,
            List<Case> cases,
            IRandomGenerator randomGenerator,
            string sipAddressStem)
        {
            var newParticipants = MapParticipants(request, caseType);
            var newEndpoints = MapEndpoints(request, randomGenerator, sipAddressStem);
            var linkedParticipants = MapLinkedParticipants(request);

            return new CreateVideoHearingCommand(caseType, hearingType,
                request.ScheduledDateTime, request.ScheduledDuration, venue, newParticipants, cases,
                request.QuestionnaireNotRequired, request.AudioRecordingRequired, newEndpoints, linkedParticipants, request.IsMultiDayHearing)
            {
                HearingRoomName = request.HearingRoomName,
                OtherInformation = request.OtherInformation,
                CreatedBy = request.CreatedBy
            };
        }

        private static List<NewParticipant> MapParticipants(BookNewHearingRequest request, CaseType caseType)
        {
            var newParticipants = request.Participants.Select(
                x => ParticipantRequestToNewParticipantMapper.Map(x, caseType)).ToList();
            return newParticipants;
        }

        private static List<NewEndpoint> MapEndpoints(BookNewHearingRequest request, IRandomGenerator randomGenerator,
            string sipAddressStem)
        {
            var endpoints = new List<NewEndpoint>();
            if (request.Endpoints != null)
            {
                endpoints = request.Endpoints.Select(x =>
                    EndpointToResponseMapper.MapRequestToNewEndpointDto(x, randomGenerator, sipAddressStem)).ToList();
            }

            return endpoints;
        }

        private static List<LinkedParticipantDto> MapLinkedParticipants(BookNewHearingRequest request)
        {
            var dto = LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants)
                .ToList();
            return dto;
        }
    }
}