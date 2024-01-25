using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface INewJudiciaryAddedAsynchronousProcesses
    {
        Task Start(VideoHearing videoHearing, IList<JudiciaryParticipant> newJudiciaryParticipants);
    }

    public class NewJudiciaryAddedAsynchronousProcesses : INewJudiciaryAddedAsynchronousProcesses
    {
        private readonly IEventPublisherFactory _publisherFactory;

        public NewJudiciaryAddedAsynchronousProcesses(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public async Task Start(VideoHearing videoHearing, IList<JudiciaryParticipant> newJudiciaryParticipants)
        {
            await ((IPublishJudiciaryParticipantsEvent)_publisherFactory.Get(EventType.JudiciaryParticipantAddedEvent)).PublishAsync(videoHearing, videoHearing.GetJudiciaryParticipants());
            await _publisherFactory.Get(EventType.HearingNotificationForJudiciaryParticipantEvent)
                .PublishAsync(videoHearing);
        }
    }
}