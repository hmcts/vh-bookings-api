using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class HearingNotificationEventForJudiciaryParticipantPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public HearingNotificationEventForJudiciaryParticipantPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.HearingNotificationForJudiciaryParticipantEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var videoHearingUpdateDate = videoHearing.UpdatedDate.TrimSeconds();
            // we need to send a hearing confirmation for new Panel Member created for V1 and send new templates. A create email for those users
            // has been sent previously with login details and needs a second email for hearing confirmation
            var newJudicialOfficers = PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing, videoHearingUpdateDate).Where(x => x is JudicialOfficeHolder);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newJudicialOfficers)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case)));
            }
            foreach (var participant in videoHearing.JudiciaryParticipants)
            {
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
        }
    }
}
