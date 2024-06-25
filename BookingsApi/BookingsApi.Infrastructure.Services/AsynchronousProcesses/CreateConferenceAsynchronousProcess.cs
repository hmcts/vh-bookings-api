using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
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
