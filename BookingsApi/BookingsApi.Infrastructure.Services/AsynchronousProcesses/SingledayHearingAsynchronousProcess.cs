using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public class SingledayHearingAsynchronousProcess : IBookingAsynchronousProcess
    {
        private readonly IEventPublisher _eventPublisher;
        public SingledayHearingAsynchronousProcess(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            await SendWelcomeEmailForNewParticipants(videoHearing);
            await CreateConferenceEvent(videoHearing);
            await HearingConfirmationforNewParticipants(videoHearing);
            await HearingConfirmationforExistingParticipants(videoHearing);
        }

        private async Task HearingConfirmationforExistingParticipants(VideoHearing videoHearing)
        {
            var existingParticipants = videoHearing.Participants.Where(x => x.DoesPersonAlreadyExist());

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in existingParticipants)
            {
                await _eventPublisher.PublishAsync(new ExistingParticipantHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
        }

        private async Task HearingConfirmationforNewParticipants(VideoHearing videoHearing)
        {
            var newParticipants = videoHearing.Participants.Where(x => !x.DoesPersonAlreadyExist());

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                await _eventPublisher.PublishAsync(new NewParticipantHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
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
