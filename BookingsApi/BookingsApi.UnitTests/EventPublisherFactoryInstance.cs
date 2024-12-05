﻿using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Collections.Generic;

namespace BookingsApi.UnitTests
{
    public class EventPublisherFactoryInstance
    {
        protected Mock<IEventPublisher> EventPublisherMock;
        public static EventPublisherFactory Get(IEventPublisher eventPublisher)
        {
            return new EventPublisherFactory(new List<IPublishEvent> {
                new WelcomeEmailForNewParticipantsPublisher(eventPublisher),
                new CreateConferencePublisher(eventPublisher),
                new HearingConfirmationforNewParticipantsPublisher(eventPublisher),
                new HearingConfirmationforExistingParticipantsPublisher(eventPublisher),
                new MultidayHearingConfirmationforNewParticipantsPublisher(eventPublisher),
                new MultidayHearingConfirmationforExistingParticipantsPublisher(eventPublisher),
                new HearingNotificationEventForJudiciaryParticipantPublisher(eventPublisher),
                new JudiciaryParticipantAddedPublisher(eventPublisher)
            });
        }
    }
}
