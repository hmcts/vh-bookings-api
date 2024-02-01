using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Domain;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public interface IPublishMultidayUpdateEvent : IPublishEvent
    {
        Task PublishAsync(VideoHearing videoHearing, IList<VideoHearing> multiDayHearings);
    }
}
