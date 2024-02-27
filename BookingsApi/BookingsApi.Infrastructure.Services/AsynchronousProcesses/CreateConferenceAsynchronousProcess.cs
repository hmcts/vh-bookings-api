using BookingsApi.Common;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface ICreateConferenceAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class CreateConferenceAsynchronousProcess : ICreateConferenceAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        public CreateConferenceAsynchronousProcess(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
        }
    }
}
