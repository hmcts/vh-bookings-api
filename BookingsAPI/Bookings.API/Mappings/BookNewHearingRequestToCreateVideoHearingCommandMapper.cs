using System.Collections.Generic;
using System.Linq;
using Bookings.Api.Contract.Requests;
using Bookings.Common;
using Bookings.Common.Configuration;
using Bookings.Common.Services;
using Bookings.DAL.Commands;
using Bookings.DAL.Dtos;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Microsoft.ApplicationInsights.DataContracts;

namespace Bookings.API.Mappings
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
            KinlyConfiguration kinlyConfiguration,
            ILogger logger)
        {
            var newParticipants = MapParticipants(request, caseType, logger);
            var newEndpoints = MapEndpoints(request, randomGenerator, kinlyConfiguration, logger);
            var linkedParticipants = MapLinkedParticipants(request, logger);

            return new CreateVideoHearingCommand(caseType, hearingType,
                request.ScheduledDateTime, request.ScheduledDuration, venue, newParticipants, cases,
                request.QuestionnaireNotRequired, request.AudioRecordingRequired, newEndpoints, linkedParticipants)
            {
                HearingRoomName = request.HearingRoomName,
                OtherInformation = request.OtherInformation,
                CreatedBy = request.CreatedBy
            };
        }

        private static List<NewParticipant> MapParticipants(BookNewHearingRequest request, CaseType caseType, ILogger logger)
        {
            var newParticipants = request.Participants.Select(
                x => ParticipantRequestToNewParticipantMapper.Map(x, caseType)).ToList();
            const string logMappedParticipants = "BookNewHearing mapped participants";
            const string keyParticipants = "Participants";
            logger.TrackTrace(logMappedParticipants, SeverityLevel.Information, new Dictionary<string, string>
            {
                {keyParticipants, string.Join(", ", newParticipants?.Select(x => x?.Person?.Username))}
            });

            return newParticipants;
        }

        private static List<NewEndpoint> MapEndpoints(BookNewHearingRequest request, IRandomGenerator randomGenerator, KinlyConfiguration kinlyConfiguration, ILogger logger)
        {
            var endpoints = new List<NewEndpoint>();
            if (request.Endpoints != null)
            {
                endpoints = request.Endpoints.Select(x =>
                    EndpointToResponseMapper.MapRequestToNewEndpointDto(x, randomGenerator,
                        kinlyConfiguration.SipAddressStem)).ToList();

                const string logMappedEndpoints = "BookNewHearing mapped endpoints";
                const string keyEndpoints = "Endpoints";
                logger.TrackTrace(logMappedEndpoints, SeverityLevel.Information, new Dictionary<string, string>
                {
                    {keyEndpoints, string.Join(", ", endpoints?.Select(x => new {x?.Sip, x?.DisplayName, x?.DefenceAdvocateUsername}))}
                });
            }

            return endpoints;
        }

        private static List<LinkedParticipantDto> MapLinkedParticipants(BookNewHearingRequest request, ILogger logger)
        {
            var dto = LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants).ToList();
            
            logger.TrackTrace("Linked participants mapped to dto", SeverityLevel.Information, new Dictionary<string, string>
            {
                {"LinkedParticipants", string.Join(", ", dto.Select(x => new {x?.ParticipantContactEmail, x?.LinkedParticipantContactEmail, x?.Type}))}
            });

            return dto;
        }
    }
}