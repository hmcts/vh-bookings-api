using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;

namespace BookingsApi.UnitTests.Infrastructure.Services.Publishers
{
    public class JudiciaryParticipantAddedPublisherTests
    {
        private JudiciaryParticipantAddedPublisher _publisher;
        private Mock<IEventPublisher> _mockEventPublisher;
        
        [SetUp]
        public void Setup()
        {
            _mockEventPublisher = new Mock<IEventPublisher>();
            _publisher = new JudiciaryParticipantAddedPublisher(_mockEventPublisher.Object);
        }
        
        [Test]
        public async Task Should_publish_event_with_judiciary_participants_provided()
        {
            // Arrange
            var hearing1 = new VideoHearingBuilder(addJudge: false)
                .WithCase()
                .Build();
            
            var hearing2 = new VideoHearingBuilder(addJudge: false)
                .WithJudiciaryJudge()
                .WithJudiciaryPanelMember()
                .Build();

            var judiciaryParticipants = hearing2.GetJudiciaryParticipants();
            
            // Act
            await _publisher.PublishAsync(hearing1, judiciaryParticipants);

            // Assert
            _mockEventPublisher.Verify(p => p.PublishAsync(It.Is<ParticipantsAddedIntegrationEvent>(
                    e => e.Hearing.HearingId == hearing1.Id && 
                         e.Participants.Count == judiciaryParticipants.Count)), 
                Times.Once);
        }
        
        [Test]
        public async Task Should_publish_event_with_no_judiciary_participants_provided()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false)
                .WithCase()
                .WithJudiciaryJudge()
                .WithJudiciaryPanelMember()
                .Build();

            var judiciaryParticipants = hearing.GetJudiciaryParticipants();
            
            // Act
            await _publisher.PublishAsync(hearing);

            // Assert
            _mockEventPublisher.Verify(p => p.PublishAsync(It.Is<ParticipantsAddedIntegrationEvent>(
                    e => e.Hearing.HearingId == hearing.Id && 
                         e.Participants.Count == judiciaryParticipants.Count)), 
                Times.Once);
        }
    }
}
