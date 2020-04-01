using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingIsReadyForVideoIntegrationEvent : IIntegrationEvent
    {
        public HearingIsReadyForVideoIntegrationEvent(Hearing hearing)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            var hearingParticipants = hearing.GetParticipants();
            Participants = hearingParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
        }

        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
    }
}

    