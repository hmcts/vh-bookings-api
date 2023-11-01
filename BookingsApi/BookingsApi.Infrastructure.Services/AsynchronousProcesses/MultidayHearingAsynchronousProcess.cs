using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IClonedBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing, int totalDays);
    }

    public class ClonedMultidaysAsynchronousProcess: IClonedBookingAsynchronousProcess
    {
        private readonly IEventPublisher _eventPublisher;
        public ClonedMultidaysAsynchronousProcess(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Start(VideoHearing videoHearing, int totalDays)
        {
            if(totalDays <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalDays));
            }

            await MultidayHearingConfirmationforNewParticipants(videoHearing, totalDays);
            await MultidayHearingConfirmationforExistingParticipants(videoHearing, totalDays);
        }

        private async Task MultidayHearingConfirmationforExistingParticipants(VideoHearing videoHearing, int totaldays)
        {
            var existingParticipants = videoHearing.Participants.Where(x => x.DoesPersonAlreadyExist());
            var @case = videoHearing.GetCases()[0];
            foreach (var participant in existingParticipants)
            {
                await _eventPublisher.PublishAsync(new ExistingParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case), totaldays));
            }
        }

        private async Task MultidayHearingConfirmationforNewParticipants(VideoHearing videoHearing, int totaldays)
        {
            var newParticipants = videoHearing.Participants.Where(x => !x.DoesPersonAlreadyExist());
            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                await _eventPublisher.PublishAsync(new NewParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case), totaldays));
            }
        }
    }
}
