using System.Linq;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface INewJudiciaryAddedAsynchronousProcesses
    {
        Task Start(VideoHearing videoHearing);
    }

    public class NewJudiciaryAddedAsynchronousProcesses : INewJudiciaryAddedAsynchronousProcesses
    {
        private readonly IEventPublisherFactory _publisherFactory;

        public NewJudiciaryAddedAsynchronousProcesses(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            await _publisherFactory.Get(EventType.ParticipantAddedEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.HearingNotificationForJudiciaryParticipantEvent)
                .PublishAsync(videoHearing);
        }
    }
}