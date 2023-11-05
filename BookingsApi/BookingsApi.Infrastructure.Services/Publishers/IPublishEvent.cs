using BookingsApi.Domain;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public interface IPublishEvent
    {
        Task PublishAsync(VideoHearing videoHearing);
        EventType EventType { get; }
    }
}
