using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class WelcomeEmailForNewParticipantsPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public WelcomeEmailForNewParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.NewParticipantWelcomeEmailEvent;
        
        public IList<Guid> ParticipantsToBeNotifiedIds { get; set; }

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            if (ParticipantsToBeNotifiedIds == null)
                ParticipantsToBeNotifiedIds = videoHearing.Participants.Select(p => p.Id).ToList();
            var newParticipants = PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing, ParticipantsToBeNotifiedIds).Where(x => x is not JudicialOfficeHolder);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new NewParticipantWelcomeEmailEvent(EventDtoMappers.MapToWelcomeEmailDto(
                    videoHearing.Id, participantDto, @case)));
            }
        }
    }
}
