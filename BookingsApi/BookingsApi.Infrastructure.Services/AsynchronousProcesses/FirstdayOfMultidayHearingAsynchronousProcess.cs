using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IFirstdayOfMultidayBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class FirstdayOfMultidayHearingAsynchronousProcess : IFirstdayOfMultidayBookingAsynchronousProcess
    {
        private readonly IEventPublisher _eventPublisher;
        public FirstdayOfMultidayHearingAsynchronousProcess(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            await SendWelcomeEmailForNewParticipants(videoHearing);
            await CreateConferenceEvent(videoHearing);
        }

        private async Task CreateConferenceEvent(VideoHearing videoHearing)
        {
            await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(videoHearing, videoHearing.Participants));
        }

        private async Task SendWelcomeEmailForNewParticipants(VideoHearing videoHearing)
        {
            var newParticipants = videoHearing.Participants.Where(x => x is Representative && !x.DoesPersonAlreadyExist());

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                await _eventPublisher.PublishAsync(new NewParticipantWelcomeEmailEvent(EventDtoMappers.MapToWelcomeEmailDto(
                    videoHearing.Id, participant, @case)));
            }
        }
    }
}
