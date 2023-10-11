using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
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

           return new CreateVideoHearingCommand(
                new CreateVideoHearingRequiredDto(caseType, request.ScheduledDateTime,
                    request.ScheduledDuration, venue, cases),
                new CreateVideoHearingOptionalDto(newParticipants, request.HearingRoomName, request.OtherInformation, request.CreatedBy,
                    request.AudioRecordingRequired, newEndpoints, null, linkedParticipants,
                    new List<NewJudiciaryParticipant>(), request.IsMultiDayHearing, null, hearingType)
            );
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