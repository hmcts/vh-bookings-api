using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Infrastructure.Services.Publishers;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public interface IEventPublisherFactory
    {
        IPublishEvent Get(EventType eventType);
    }

    public class EventPublisherFactory : IEventPublisherFactory
    {
        private readonly IList<IPublishEvent> _eventPublishers;
        public EventPublisherFactory(IList<IPublishEvent> eventPublishers)
        {
            _eventPublishers = eventPublishers;
        }

        public IPublishEvent Get(EventType eventType)
        {
            return _eventPublishers.Single(x => x.EventType == eventType);
        }
    }
}
