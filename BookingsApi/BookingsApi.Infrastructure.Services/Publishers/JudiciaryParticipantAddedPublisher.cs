using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class JudiciaryParticipantAddedPublisher : IPublishJudiciaryParticipantsEvent
    {
        private readonly IEventPublisher _eventPublisher;
        
        public JudiciaryParticipantAddedPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.JudiciaryParticipantAddedEvent;
        
        public async Task PublishAsync(VideoHearing videoHearing, IList<JudiciaryParticipant> judiciaryParticipants)
        {
            await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(videoHearing, judiciaryParticipants));
        }

        /// <remarks>
        /// This overload is purely to comply with the base interface. Do not use
        /// </remarks>
        /// <deprecated>Use the overload with judiciaryParticipants instead</deprecated>
        public Task PublishAsync(VideoHearing videoHearing)
        {
            throw new InvalidOperationException();
        }
    }
}
