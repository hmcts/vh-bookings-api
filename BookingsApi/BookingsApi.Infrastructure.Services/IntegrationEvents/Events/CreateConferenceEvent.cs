using BookingsApi.Infrastructure.Services.Dtos;
using System;
using System.Collections.Generic;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class CreateConferenceEvent
    {
        public Guid HearingId { get; set; }
        public bool IsAudioRecordingRequired { get; set; }
        public string AudioRecordingURI { get; set; }

        public IList<ParticipantDto> Participants { get; }
        public IList<EndpointDto> Endpoints { get;  }
    }
}
