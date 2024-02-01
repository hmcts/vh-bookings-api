using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryParticipants
{
    public abstract class JudiciaryParticipantApiTest : ApiTest
    {
        protected void AssertEventsPublishedForNewJudiciaryParticipants(Hearing hearing, JudiciaryParticipant judiciaryParticipant)
        {
            AssertEventsPublishedForNewJudiciaryParticipants(hearing, new List<JudiciaryParticipant>{ judiciaryParticipant });
        }
        
        protected void AssertEventsPublishedForNewJudiciaryParticipants(Hearing hearing, IEnumerable<JudiciaryParticipant> judiciaryParticipants)
        {
            var serviceBusStub = Application.Services
                .GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
            var messages = serviceBusStub!
                .ReadAllMessagesFromQueue(hearing.Id);
            
            var participantAddedMessage = messages.ToList().Find(x => x.IntegrationEvent is ParticipantsAddedIntegrationEvent);
            participantAddedMessage.Should().NotBeNull();
            participantAddedMessage.IntegrationEvent
                .Should()
                .BeEquivalentTo(new ParticipantsAddedIntegrationEvent(hearing, judiciaryParticipants));
        }
    }
}
