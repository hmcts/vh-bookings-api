using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingIsReadyForVideoIntegrationEvent : IIntegrationEvent
    {
        public HearingIsReadyForVideoIntegrationEvent(Hearing hearing)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            
            var hearingParticipants = hearing.GetParticipants();
            Participants = hearingParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            Participants.ToList().ForEach(x =>
                x.SetContactEmailForNonEJudJudgeUser(hearing.OtherInformation));
            var hearingEndpoints = hearing.GetEndpoints();
            Endpoints = hearingEndpoints.Select(EndpointDtoMapper.MapToDto).ToList();
        }

        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
        public IList<EndpointDto> Endpoints { get; set; }
    }
}

    